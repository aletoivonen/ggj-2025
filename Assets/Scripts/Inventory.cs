namespace Zubble
{
    public class Inventory
    {
        public static Inventory Instance => _inventory ??= new Inventory();
        private static Inventory _inventory;

        public float Soap { get; private set; }

        public Inventory()
        {
            if (_inventory != null)
            {
                return;
            }
            _inventory = this;
        }

        public void AddSoap(float amount)
        {
            Soap += amount;
        }

        public void RemoveSoap(float amount)
        {
            Soap -= amount;
        }
    }
}
