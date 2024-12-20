using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kashapov
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Photo_Number { get; set; }
        public string ImagePath => GetImagePath(Photo_Number);

        private string GetImagePath(int photoNumber)
        {
            return photoNumber switch
            {
                2 => "image1.png",
                1 => "image2.png",
                3 => "image3.png",
                _ => "placeholder.png", // Изображение по умолчанию
            };
        }
    }

}
