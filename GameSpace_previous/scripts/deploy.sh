#!/bin/bash

# GameSpace 部署腳本
# 用法: ./deploy.sh [environment] [action]
# 環境: dev, staging, prod
# 動作: build, deploy, rollback, status

set -e

# 顏色定義
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# 配置
PROJECT_NAME="gamespace"
DOCKER_REGISTRY="your-registry.com"
VERSION=$(git describe --tags --always --dirty)
ENVIRONMENT=${1:-dev}
ACTION=${2:-deploy}

# 日誌函數
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# 檢查依賴
check_dependencies() {
    log_info "檢查依賴..."
    
    if ! command -v docker &> /dev/null; then
        log_error "Docker 未安裝"
        exit 1
    fi
    
    if ! command -v kubectl &> /dev/null; then
        log_error "kubectl 未安裝"
        exit 1
    fi
    
    if ! command -v dotnet &> /dev/null; then
        log_error ".NET SDK 未安裝"
        exit 1
    fi
    
    log_success "所有依賴已安裝"
}

# 構建應用程式
build_application() {
    log_info "構建應用程式..."
    
    # 清理舊的構建
    log_info "清理舊的構建文件..."
    dotnet clean
    rm -rf bin/ obj/
    
    # 還原依賴
    log_info "還原 NuGet 包..."
    dotnet restore
    
    # 運行測試
    log_info "運行單元測試..."
    dotnet test --no-build --configuration Release --verbosity normal
    
    # 構建應用程式
    log_info "構建應用程式..."
    dotnet build --configuration Release --no-restore
    
    # 發布應用程式
    log_info "發布應用程式..."
    dotnet publish --configuration Release --no-build --output ./publish
    
    log_success "應用程式構建完成"
}

# 構建 Docker 映像
build_docker_image() {
    log_info "構建 Docker 映像..."
    
    local image_tag="${DOCKER_REGISTRY}/${PROJECT_NAME}:${VERSION}"
    local latest_tag="${DOCKER_REGISTRY}/${PROJECT_NAME}:latest"
    
    # 構建映像
    docker build -t "${image_tag}" -t "${latest_tag}" .
    
    # 推送到註冊表
    log_info "推送映像到註冊表..."
    docker push "${image_tag}"
    docker push "${latest_tag}"
    
    log_success "Docker 映像構建並推送完成: ${image_tag}"
}

# 部署到 Kubernetes
deploy_to_kubernetes() {
    log_info "部署到 Kubernetes..."
    
    # 更新映像標籤
    sed -i "s|image: gamespace:.*|image: ${DOCKER_REGISTRY}/${PROJECT_NAME}:${VERSION}|g" k8s/deployment.yaml
    
    # 應用 Kubernetes 配置
    log_info "應用 Kubernetes 配置..."
    kubectl apply -f k8s/namespace.yaml
    kubectl apply -f k8s/configmap.yaml
    kubectl apply -f k8s/secret.yaml
    kubectl apply -f k8s/deployment.yaml
    kubectl apply -f k8s/service.yaml
    kubectl apply -f k8s/ingress.yaml
    
    # 等待部署完成
    log_info "等待部署完成..."
    kubectl rollout status deployment/gamespace-app -n gamespace --timeout=300s
    
    log_success "部署到 Kubernetes 完成"
}

# 部署到 Docker Compose
deploy_to_docker_compose() {
    log_info "部署到 Docker Compose..."
    
    # 停止舊的容器
    log_info "停止舊的容器..."
    docker-compose down
    
    # 拉取最新映像
    log_info "拉取最新映像..."
    docker-compose pull
    
    # 啟動服務
    log_info "啟動服務..."
    docker-compose up -d
    
    # 等待服務啟動
    log_info "等待服務啟動..."
    sleep 30
    
    # 檢查服務狀態
    log_info "檢查服務狀態..."
    docker-compose ps
    
    log_success "Docker Compose 部署完成"
}

# 回滾部署
rollback_deployment() {
    log_info "回滾部署..."
    
    if [ "$ENVIRONMENT" = "kubernetes" ]; then
        kubectl rollout undo deployment/gamespace-app -n gamespace
        kubectl rollout status deployment/gamespace-app -n gamespace --timeout=300s
    else
        docker-compose down
        docker-compose up -d
    fi
    
    log_success "回滾完成"
}

# 檢查部署狀態
check_status() {
    log_info "檢查部署狀態..."
    
    if [ "$ENVIRONMENT" = "kubernetes" ]; then
        log_info "Kubernetes 狀態:"
        kubectl get pods -n gamespace
        kubectl get services -n gamespace
        kubectl get ingress -n gamespace
        
        log_info "應用程式日誌:"
        kubectl logs -l app=gamespace -n gamespace --tail=50
    else
        log_info "Docker Compose 狀態:"
        docker-compose ps
        
        log_info "應用程式日誌:"
        docker-compose logs --tail=50
    fi
}

# 健康檢查
health_check() {
    log_info "執行健康檢查..."
    
    local max_attempts=30
    local attempt=1
    
    while [ $attempt -le $max_attempts ]; do
        log_info "健康檢查嘗試 $attempt/$max_attempts..."
        
        if curl -f http://localhost/health > /dev/null 2>&1; then
            log_success "健康檢查通過"
            return 0
        fi
        
        sleep 10
        ((attempt++))
    done
    
    log_error "健康檢查失敗"
    return 1
}

# 清理資源
cleanup() {
    log_info "清理資源..."
    
    # 清理未使用的 Docker 映像
    docker image prune -f
    
    # 清理未使用的容器
    docker container prune -f
    
    log_success "資源清理完成"
}

# 主函數
main() {
    log_info "開始 GameSpace 部署流程..."
    log_info "環境: $ENVIRONMENT"
    log_info "動作: $ACTION"
    log_info "版本: $VERSION"
    
    case $ACTION in
        "build")
            check_dependencies
            build_application
            build_docker_image
            ;;
        "deploy")
            check_dependencies
            build_application
            build_docker_image
            
            if [ "$ENVIRONMENT" = "kubernetes" ]; then
                deploy_to_kubernetes
            else
                deploy_to_docker_compose
            fi
            
            health_check
            ;;
        "rollback")
            rollback_deployment
            health_check
            ;;
        "status")
            check_status
            ;;
        "cleanup")
            cleanup
            ;;
        *)
            log_error "未知的動作: $ACTION"
            log_info "可用的動作: build, deploy, rollback, status, cleanup"
            exit 1
            ;;
    esac
    
    log_success "GameSpace 部署流程完成"
}

# 錯誤處理
trap 'log_error "部署過程中發生錯誤，退出碼: $?"' ERR

# 執行主函數
main "$@"