namespace Products.Models{
    public class Combo
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string image { get; set; } = string.Empty;
        public List<Product> products { get; set; } = new();
        public decimal price { get; set; } = 0.0M;
        public bool active { get; set; } = true;

        public override string ToString()
        {
            return $"ID: {id}, {name} con precio {price.ToString("C2")} y cantidad {products.Count}";
        }
    }
}