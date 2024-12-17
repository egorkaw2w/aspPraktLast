using Microsoft.AspNetCore.Mvc;
using aspPrakt.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace aspPrakt.Controllers
{
    public class AdminController : Controller
    {
        private readonly BpnContext _context;

        public AdminController(BpnContext context)
        {
            _context = context;
        }

        // Отображение данных таблицы
        public async Task<IActionResult> Table(string tableName)
        {
            var data = await GetTableData(tableName);
            ViewBag.TableName = tableName;
            return View(data);
        }

        // Добавление записи
        [HttpPost]
        public async Task<IActionResult> Add(string tableName, [FromForm] dynamic model)
        {
            try
            {
                await AddOrUpdateRecord(tableName, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при добавлении записи: {ex.Message}");
            }
        }

        // Редактирование записи
        [HttpPost]
        public async Task<IActionResult> Edit(string tableName, int id, [FromForm] dynamic model)
        {
            try
            {
                await AddOrUpdateRecord(tableName, model);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при редактировании записи: {ex.Message}");
            }
        }

        // Удаление записи
        [HttpPost]
        public async Task<IActionResult> Delete(string tableName, int id)
        {
            try
            {
                await DeleteRecord(tableName, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка при удалении записи: {ex.Message}");
            }
        }

        // Вспомогательные методы
        private async Task<List<dynamic>> GetTableData(string tableName)
        {
            switch (tableName)
            {
                case "Roles":
                    return await _context.Roles.ToListAsync<dynamic>();
                case "Client":
                    return await _context.Clients.ToListAsync<dynamic>();
                case "Categories":
                    return await _context.Categories.ToListAsync<dynamic>();
                case "Products":
                    return await _context.Products.ToListAsync<dynamic>();
                case "Orders":
                    return await _context.Orders.ToListAsync<dynamic>();
                case "OrderItems":
                    return await _context.OrderItems.ToListAsync<dynamic>();
                case "CartItems":
                    return await _context.CartItems.ToListAsync<dynamic>();
                default:
                    return new List<dynamic>();
            }
        }

        private async Task AddOrUpdateRecord(string tableName, dynamic model)
        {
            switch (tableName)
            {
                case "Roles":
                    if (model.RoleId == 0)
                        _context.Roles.Add(model);
                    else
                        _context.Roles.Update(model);
                    break;
                case "Client":
                    if (model.UserId == 0)
                        _context.Clients.Add(model);
                    else
                        _context.Clients.Update(model);
                    break;
                case "Categories":
                    if (model.CategoryId == 0)
                        _context.Categories.Add(model);
                    else
                        _context.Categories.Update(model);
                    break;
                case "Products":
                    if (model.ProductId == 0)
                        _context.Products.Add(model);
                    else
                        _context.Products.Update(model);
                    break;
                case "Orders":
                    if (model.OrderId == 0)
                        _context.Orders.Add(model);
                    else
                        _context.Orders.Update(model);
                    break;
                case "OrderItems":
                    if (model.OrderItemId == 0)
                        _context.OrderItems.Add(model);
                    else
                        _context.OrderItems.Update(model);
                    break;
                case "CartItems":
                    if (model.CartItemId == 0)
                        _context.CartItems.Add(model);
                    else
                        _context.CartItems.Update(model);
                    break;
            }
            await _context.SaveChangesAsync();
        }

        private async Task DeleteRecord(string tableName, int id)
        {
            var record = await GetRecordById(tableName, id);
            if (record != null)
            {
                switch (tableName)
                {
                    case "Roles":
                        _context.Roles.Remove(record);
                        break;
                    case "Client":
                        _context.Clients.Remove(record);
                        break;
                    case "Categories":
                        _context.Categories.Remove(record);
                        break;
                    case "Products":
                        _context.Products.Remove(record);
                        break;
                    case "Orders":
                        _context.Orders.Remove(record);
                        break;
                    case "OrderItems":
                        _context.OrderItems.Remove(record);
                        break;
                    case "CartItems":
                        _context.CartItems.Remove(record);
                        break;
                }
                await _context.SaveChangesAsync();
            }
        }

        private async Task<dynamic> GetRecordById(string tableName, int id)
        {
            switch (tableName)
            {
                case "Roles":
                    return await _context.Roles.FindAsync(id);
                case "Client":
                    return await _context.Clients.FindAsync(id);
                case "Categories":
                    return await _context.Categories.FindAsync(id);
                case "Products":
                    return await _context.Products.FindAsync(id);
                case "Orders":
                    return await _context.Orders.FindAsync(id);
                case "OrderItems":
                    return await _context.OrderItems.FindAsync(id);
                case "CartItems":
                    return await _context.CartItems.FindAsync(id);
                default:
                    return null;
            }
        }
    }
}