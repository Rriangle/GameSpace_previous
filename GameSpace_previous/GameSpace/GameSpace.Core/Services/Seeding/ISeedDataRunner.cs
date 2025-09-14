using System.Threading.Tasks;

namespace GameSpace.Core.Services.Seeding
{
    /// <summary>
    /// 種子數據運行器介面
    /// </summary>
    public interface ISeedDataRunner
    {
        /// <summary>
        /// 執行種子數據初始化
        /// </summary>
        Task SeedAsync();
        
        /// <summary>
        /// 檢查是否需要執行種子數據
        /// </summary>
        Task<bool> NeedsSeedingAsync();
    }
}
