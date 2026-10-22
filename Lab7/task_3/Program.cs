using System;
using System.Collections.Generic;
using System.Linq;

namespace Variant6_VehicleFleet
{
    // Опис предметної області: Автоматизована система управління транспортним парком.
    // Завдання коду: Заправка, технічний огляд, вимірювання залишку палива та пробігу транспортних засобів.

    //  Незмінний Value Object: ємність + поточний рівень (літри палива або кВт·год заряду) 
    // Інваріант: 0 <= Level <= Capacity. Створити некоректний стан неможливо (fail-fast).

    public sealed record Reservoir
    {
        public double Capacity { get; }
        public double Level { get; }
        public double FreeSpace => Capacity - Level;

        public Reservoir(double capacity, double level)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
            }

            if (level < 0 || level > capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be between 0 and capacity.");
            }

            Capacity = capacity;
            Level = level;
        }

        public Reservoir Add(double amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(amount));
            if (amount > FreeSpace) throw new InvalidOperationException("Capacity exceeded!");

            return new Reservoir(Capacity, Math.Min(Capacity, Level + amount));
        }

        public Reservoir Remove(double amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(amount));
            if (amount > Level) throw new InvalidOperationException("Not enough energy!");

            return new Reservoir(Capacity, Math.Max(0, Level - amount));
        }
    }

    // Базовий тип: лише те, що вміє КОЖЕН транспортний засіб

    public abstract class Vehicle
    {
        public string LicensePlate { get; }

        protected Vehicle(string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(licensePlate))
            {
                throw new ArgumentException("License plate is required.", nameof(licensePlate));
            }

            LicensePlate = licensePlate;
        }

        public abstract void Drive(double distanceKm);

        protected static void EnsurePositiveDistance(double distanceKm)
        {
            if (distanceKm <= 0) throw new ArgumentException("Distance must be positive.", nameof(distanceKm));
        }
    }

    // Паливні ТЗ: мають бак і вміють заправлятися

    public class FuelVehicle : Vehicle
    {
        private const double ConsumptionLitersPerKm = 0.08; // 8 л/100 км

        private Reservoir _tank;

        public double FuelLevelLiters => _tank.Level;
        public double TankCapacityLiters => _tank.Capacity;

        public FuelVehicle(string licensePlate, double fuelLevelLiters, double tankCapacityLiters)
            : base(licensePlate)
        {
            _tank = new Reservoir(tankCapacityLiters, fuelLevelLiters);
        }

        public void Refuel(double liters)
        {
            _tank = _tank.Add(liters);
            Console.WriteLine($"Vehicle {LicensePlate} refueled with {liters} L. Current level: {FuelLevelLiters} L");
        }

        public override void Drive(double distanceKm)
        {
            EnsurePositiveDistance(distanceKm);
            _tank = _tank.Remove(distanceKm * ConsumptionLitersPerKm);
            Console.WriteLine($"Vehicle {LicensePlate} drove {distanceKm} km. Remaining fuel: {FuelLevelLiters} L");
        }
    }

    // Електромобілі: мають батарею, а не бак. Не успадковують Refuel

    public class ElectricVehicle : Vehicle
    {
        private const double ConsumptionKWhPerKm = 0.15; // 15 кВт·год/100 км

        private Reservoir _battery;

        public double BatteryLevelKWh => _battery.Level;

        public ElectricVehicle(string licensePlate, double batteryLevelKWh, double batteryCapacityKWh)
            : base(licensePlate)
        {
            _battery = new Reservoir(batteryCapacityKWh, batteryLevelKWh);
        }

        public void ChargeBattery(double kWh)
        {
            _battery = _battery.Add(kWh);
            Console.WriteLine($"Vehicle {LicensePlate} charged with {kWh} kWh. Current level: {BatteryLevelKWh} kWh");
        }

        public override void Drive(double distanceKm)
        {
            EnsurePositiveDistance(distanceKm);
            _battery = _battery.Remove(distanceKm * ConsumptionKWhPerKm);
            Console.WriteLine($"Vehicle {LicensePlate} drove {distanceKm} km. Remaining charge: {BatteryLevelKWh} kWh");
        }
    }

    // Клієнт: приймає лише ті типи, які справді можна заправити

    public class GasStationManager
    {
        public void RefuelVehicleToFull(FuelVehicle vehicle)
        {
            ArgumentNullException.ThrowIfNull(vehicle);

            double needed = vehicle.TankCapacityLiters - vehicle.FuelLevelLiters;
            if (needed > 0)
            {
                vehicle.Refuel(needed);
            }
        }
    }
    public class Program
    {
        public static void Main()
        {
            var fleet = new List<Vehicle>
            {
                new FuelVehicle("AA1234BB", fuelLevelLiters: 10, tankCapacityLiters: 50),
                new ElectricVehicle("BC5678CD", batteryLevelKWh: 40, batteryCapacityKWh: 60)
            };

            // LSP: будь-який Vehicle можна підставити там, де потрібен Vehicle
            foreach (Vehicle vehicle in fleet)
            {
                vehicle.Drive(100);
            }

            // Заправляємо лише паливні, заряджаємо лише електричні
            var station = new GasStationManager();
            foreach (FuelVehicle fuelVehicle in fleet.OfType<FuelVehicle>())
            {
                station.RefuelVehicleToFull(fuelVehicle);
            }

            foreach (ElectricVehicle ev in fleet.OfType<ElectricVehicle>())
            {
                ev.ChargeBattery(35);
            }

            // Fail-fast: некоректний стан неможливо створити
            try
            {
                var broken = new FuelVehicle("XX0000XX", fuelLevelLiters: 100, tankCapacityLiters: 50);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Помилка створення: {ex.Message}");
            }
        }
    }
}