using Microsoft.AspNetCore.Mvc;
using GameSpace.Core.Repositories;
using GameSpace.Core.Models;

namespace GameSpace.Web.Controllers
{
    /// <summary>
    /// Shop MVC controller
    /// </summary>
    public class ShopController : Controller
    {
        private readonly ICommerceReadOnlyRepository _commerceRepository;

        public ShopController(ICommerceReadOnlyRepository commerceRepository)
        {
            _commerceRepository = commerceRepository;
        }

        /// <summary>
        /// Shop homepage
        /// </summary>
        public async Task<IActionResult> Index(string? productType = null, int page = 1)
        {
            try
            {
                var products = await _commerceRepository.GetProductsAsync(productType, page, 20);
                var productCount = await _commerceRepository.GetProductCountAsync(productType);
                var rankings = await _commerceRepository.GetOfficialStoreRankingsAsync("daily", 10);

                ViewBag.Products = products;
                ViewBag.ProductCount = productCount;
                ViewBag.Rankings = rankings;
                ViewBag.ProductType = productType;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)productCount / 20);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to load shop homepage: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// Product details
        /// </summary>
        public async Task<IActionResult> Product(int id)
        {
            try
            {
                var product = await _commerceRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var gameDetails = await _commerceRepository.GetGameProductDetailsAsync(id);
                var otherDetails = await _commerceRepository.GetOtherProductDetailsAsync(id);

                ViewBag.Product = product;
                ViewBag.GameDetails = gameDetails;
                ViewBag.OtherDetails = otherDetails;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to load product details: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// Rankings
        /// </summary>
        public async Task<IActionResult> Rankings(string? periodType = null)
        {
            try
            {
                var rankings = await _commerceRepository.GetOfficialStoreRankingsAsync(periodType, 50);
                
                ViewBag.Rankings = rankings;
                ViewBag.PeriodType = periodType ?? "daily";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to load rankings: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// Player market
        /// </summary>
        public async Task<IActionResult> Market(string? productType = null, int? sellerId = null, int page = 1)
        {
            try
            {
                var products = await _commerceRepository.GetPlayerMarketProductsAsync(productType, sellerId, page, 20);
                var productCount = await _commerceRepository.GetPlayerMarketProductCountAsync(productType, sellerId);
                var rankings = await _commerceRepository.GetPlayerMarketRankingsAsync("daily", 10);

                ViewBag.Products = products;
                ViewBag.ProductCount = productCount;
                ViewBag.Rankings = rankings;
                ViewBag.ProductType = productType;
                ViewBag.SellerId = sellerId;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)productCount / 20);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to load player market: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// Player market product details
        /// </summary>
        public async Task<IActionResult> MarketProduct(int id)
        {
            try
            {
                var product = await _commerceRepository.GetPlayerMarketProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                var images = await _commerceRepository.GetPlayerMarketProductImagesAsync(id);

                ViewBag.Product = product;
                ViewBag.Images = images;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to load player market product details: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// Order management
        /// </summary>
        public async Task<IActionResult> Orders(int userId, int page = 1)
        {
            try
            {
                var orders = await _commerceRepository.GetUserOrdersAsync(userId, page, 20);
                var orderCount = await _commerceRepository.GetOrderCountAsync(userId);

                ViewBag.Orders = orders;
                ViewBag.OrderCount = orderCount;
                ViewBag.UserId = userId;
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = (int)Math.Ceiling((double)orderCount / 20);

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to load orders: {ex.Message}";
                return View();
            }
        }

        /// <summary>
        /// Order details
        /// </summary>
        public async Task<IActionResult> Order(int id)
        {
            try
            {
                var order = await _commerceRepository.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }

                var items = await _commerceRepository.GetOrderItemsAsync(id);

                ViewBag.Order = order;
                ViewBag.Items = items;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Failed to load order details: {ex.Message}";
                return View();
            }
        }
    }
}