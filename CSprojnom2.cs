using System;
using System.Collections.Generic;

namespace CapstoneProject_final
{//main class
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to JustNom!");
            Console.Write("Please enter your name: ");
            string customerName = Console.ReadLine();

            int choice = GetValidMenuChoice("Please choose an option:\n1. Pickup\n2. Delivery\nEnter your choice (1 or 2): ", 1, 2);
            if (choice == 0)
                return;

            Order order;
            if (choice == 1)
            {
                order = new Order(customerName, "Pickup");
            }
            else // choice == 2
            {
                Console.Write("Please enter your address: ");
                string address = Console.ReadLine();
                order = new Order(customerName, "Delivery", address);
            }

            int menuChoice = GetValidMenuChoice("Menu Options:\n1. Burgers\n2. Pizzas\nEnter your choice (1 or 2): ", 1, 2);
            if (menuChoice == 0)
                return;

            if (menuChoice == 1)
            {
                HandleBurgerOrder(order);
            }
            else // menuChoice == 2
            {
                HandlePizzaOrder(order);
            }
            // calculates total price
            double totalPrice = order.CalculateTotalPrice();
            double deliveryCharge = order.CalculateDeliveryCharge();

            Console.WriteLine("Order Details:");
            Console.WriteLine($"Customer Name: {order.CustomerName}");
            Console.WriteLine($"Order Type: {order.OrderType}");
            if (order.OrderType == "Delivery")
            {
                Console.WriteLine($"Delivery Address: {order.DeliveryAddress}");
            }
            Console.WriteLine("Items:");
            foreach (FoodItem item in order.Items)
            {
                Console.WriteLine($"- {item.GetDescription()}");
            }
            Console.WriteLine($"Total Price: £{totalPrice}");
            Console.WriteLine($"Delivery Charge: £{deliveryCharge}");

            SaveOrderToFile(order);
        }

        static void HandleBurgerOrder(Order order)
        {
            Console.WriteLine("Burger Options:");
            Console.WriteLine("1. Plain Burger (£3.50)");
            Console.WriteLine("2. Cheese Burger (£4.50)");
            int burgerChoice = GetValidMenuChoice("Enter your choice (1 or 2): ", 1, 2);
            if (burgerChoice == 0)
                return;

            string burgerName = burgerChoice == 1 ? "Plain Burger" : "Cheese Burger";
            double burgerPrice = burgerChoice == 1 ? 3.50 : 4.50;

            Burger burger = new Burger(burgerName, burgerPrice);

            string[] garnishes = { "Cheese", "Fried Onions", "Bacon" };
            HandleFoodCustomization(burger, garnishes);
            order.AddItem(burger);
        }

        static void HandlePizzaOrder(Order order)
        {
            Console.WriteLine("Pizza Options:");
            Console.WriteLine("1. Margherita (£8.50)");
            Console.WriteLine("2. Ham and Mushroom (£9.00)");
            int pizzaChoice = GetValidMenuChoice("Enter your choice (1 or 2): ", 1, 2);
            if (pizzaChoice == 0)
                return;

            string pizzaName = pizzaChoice == 1 ? "Margherita" : "Ham and Mushroom";
            double pizzaPrice = pizzaChoice == 1 ? 8.50 : 9.00;

            Pizza pizza = new Pizza(pizzaName, pizzaPrice);

            string[] toppings = { "Cheese", "Pepperoni", "Tomato Sauce", "Ham", "Mushroom" };
            HandleFoodCustomization(pizza, toppings);
            order.AddItem(pizza);
        }

        static void HandleFoodCustomization(FoodItem foodItem, string[] options)
        {
            Console.WriteLine("Available Options:");
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            Console.Write("Enter the option numbers separated by commas (e.g. 1,2): ");
            string input = Console.ReadLine();
            string[] selections = input.Split(',');

            foreach (string selection in selections)
            {
                if (int.TryParse(selection, out int index) && index >= 1 && index <= options.Length)
                {
                    foodItem.AddOption(options[index - 1]);
                }
                else
                {
                    Console.WriteLine("Invalid option number. Skipping option.");
                }
            }
        }

        static int GetValidMenuChoice(string message, int min, int max)
        {
            while (true)
            {
                Console.Write(message);
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice >= min && choice <= max)
                    {
                        return choice;
                    }
                }
                Console.WriteLine("Invalid choice. Please enter a valid option.");
            }
        }
        // saves order to file
        static void SaveOrderToFile(Order order)
        {
            string filePath = "order.txt";

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Customer Name: {order.CustomerName}");
                writer.WriteLine($"Order Type: {order.OrderType}");
                if (order.OrderType == "Delivery")
                {
                    writer.WriteLine($"Delivery Address: {order.DeliveryAddress}");
                }
                writer.WriteLine("Items:");
                foreach (FoodItem item in order.Items)
                {
                    writer.WriteLine($"- {item.GetDescription()}");
                }
                writer.WriteLine($"Total Price: £{order.CalculateTotalPrice()}");
                writer.WriteLine($"Delivery Charge: £{order.CalculateDeliveryCharge()}");
            }

            Console.WriteLine($"Order saved to file: {filePath}");
        }
    }

    abstract class FoodItem
    {
        private decimal price;

        public string Name { get; }
        public double BasePrice { get; }
        public List<string> Options { get; }

        public FoodItem(string name, double basePrice)
        {
            Name = name;
            BasePrice = basePrice;
            Options = new List<string>();
        }

        protected FoodItem(string name, decimal price)
        {
            Name = name;
            this.price = price;
        }

        public abstract string GetDescription();

        public void AddOption(string option)
        {
            Options.Add(option);
        }

        public abstract double CalculateAdditionalCharge();

        public virtual void AdditionalToppingPrices()
        {
            // Implementation of the method
        }
    }

    class Pizza : FoodItem
    {
        public Pizza(string name, double basePrice) : base(name, basePrice) { }

        public override string GetDescription()
        {
            return $"{Name} Pizza";
        }

        public override double CalculateAdditionalCharge()
        {
            return Options.Count * 1.00; // Assuming each topping costs £1.00
        }

        public override void AdditionalToppingPrices()
        {
            throw new NotImplementedException();
        }
    }

    class Burger : FoodItem
    {
        public Burger(string name, double basePrice) : base(name, basePrice) { }

        public override string GetDescription()
        {
            return $"{Name} Burger";
        }

        public override double CalculateAdditionalCharge()
        {
            return Options.Count * 0.50; // Assuming each garnish costs £0.50
        }
    }

    class Order
    {
        public string CustomerName { get; }
        public string OrderType { get; }
        public string DeliveryAddress { get; }
        public List<FoodItem> Items { get; }

        public Order(string customerName, string orderType, string deliveryAddress = null)
        {
            CustomerName = customerName;
            OrderType = orderType;
            DeliveryAddress = deliveryAddress;
            Items = new List<FoodItem>();
        }

        public void AddItem(FoodItem item)
        {
            Items.Add(item);
        }

        public double CalculateTotalPrice()
        {
            double totalPrice = 0;
            foreach (var item in Items)
            {
                totalPrice += item.BasePrice + item.CalculateAdditionalCharge();
            }
            return totalPrice;
        }

        public double CalculateDeliveryCharge()
        {
            // If the order is for delivery and the total price is less than £20, charge £2 for delivery
            if (OrderType == "Delivery" && CalculateTotalPrice() < 20)
            {
                return 2.00;
            }
            // If the order is for delivery and the total price is £20 or more, delivery is free
            else if (OrderType == "Delivery" && CalculateTotalPrice() >= 20)
            {
                return 0;
            }
            // For pickup orders or other cases, delivery charge is £0
            else
            {
                return 0;
            }
        }
    }
}
