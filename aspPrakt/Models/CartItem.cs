using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspPrakt.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; } // Уникальный идентификатор элемента корзины

        public int ProductID { get; set; } // Идентификатор продукта

        public string ProductName { get; set; } // Название продукта

        public decimal Price { get; set; } // Цена продукта

        public int Quantity { get; set; } // Количество продукта

        [ForeignKey("User")]
        public int UserID { get; set; } // Идентификатор пользователя, которому принадлежит элемент корзины

        public Client User { get; set; } // Навигационное свойство для связи с пользователем
    }
}