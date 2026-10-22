using System;

namespace Variant6_VehicleFleet
{
    // Опис предметної області: Автоматизована система управління транспортним парком.
    // Завдання коду: Заправка, технічний огляд, вимірювання залишку палива та пробігу транспортних засобів.
    // 

    public class Vehicle
    {
        public string LicensePlate { get; set; }
        public double FuelLevelLiters { get; protected set; }
        public double TankCapacityLiters { get; set; }

        public Vehicle(string licensePlate, double fuelLevelLiters, double tankCapacityLiters)
        {
            LicensePlate = licensePlate;
            FuelLevelLiters = fuelLevelLiters;
            TankCapacityLiters = tankCapacityLiters;
        }

        public virtual void Refuel(double liters)
        {
            if (liters <= 0) throw new ArgumentException("Liters must be positive");
            if (FuelLevelLiters + liters > TankCapacityLiters) throw new InvalidOperationException("Tank overflow!");
            FuelLevelLiters += liters;
            Console.WriteLine($"Vehicle {LicensePlate} refueled with {liters} L. Current level: {FuelLevelLiters} L");
        }

        public virtual void Drive(double distanceKm)
        {
            double fuelConsumed = distanceKm * 0.08; // 8L/100km
            if (fuelConsumed > FuelLevelLiters) throw new InvalidOperationException("Not enough fuel to drive!");
            FuelLevelLiters -= fuelConsumed;
            Console.WriteLine($"Vehicle {LicensePlate} drove {distanceKm} km. Remaining fuel: {FuelLevelLiters} L");
        }
    }

    public class ElectricVehicle : Vehicle
    {
        public double BatteryLevelKWh { get; private set; }

        public ElectricVehicle(string licensePlate, double batteryLevelKWh)
            : base(licensePlate, 0, 0) // Не має паливного бака
        {
            BatteryLevelKWh = batteryLevelKWh;
        }

        public override void Refuel(double liters)
        {
            // Порушення LSP: Електромобіль не можна заправити бензином/дизелем
            throw new NotSupportedException("Electric vehicles cannot be refueled with liquid fuel! Use ChargeBattery instead.");
        }
    }

    public class GasStationManager
    {
        public void RefuelVehicleToFull(Vehicle vehicle)
        {
            // Заправна станція розраховує на підтримку рідкого палива у будь-якого Vehicle
            double needed = vehicle.TankCapacityLiters - vehicle.FuelLevelLiters;
            if (needed > 0)
            {
                vehicle.Refuel(needed);
            }
        }
    }
}
