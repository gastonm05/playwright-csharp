using System;

namespace CarsDemo
{
    // =========================
    // 1) Interfaces (contratos)
    // =========================

    public interface IDriveable
    {
        void Drive(int km);
    }

    public interface IRefuelable
    {
        void Refuel(int liters);
    }

    // Múltiple herencia: una clase puede implementar varias interfaces
    public interface ICar : IDriveable, IRefuelable
    {
        string Model { get; }
        int FuelLevel { get; }
    }

    // =========================
    // 2) Composición (objetos dentro del auto)
    // =========================

    public sealed class Wheel // sealed: no querés que alguien herede "Wheel" para cambiar reglas
    {
        public int RimSize { get; }

        public Wheel(int rimSize) => RimSize = rimSize;

        public override string ToString() => $"{RimSize}\" wheel";
    }

    // =========================
    // 3) Abstract base class (estado + lógica común)
    // =========================

    public abstract class CarBase : ICar
    {
        // public: se puede leer desde cualquier lado
        public string Model { get; }

        // protected: solo CarBase y clases hijas (ej: BMW/Toyota)
        protected int FuelConsumptionPerKm { get; }

        // private: solo esta clase controla el estado interno
        private int _fuelLevel;

        public int FuelLevel => _fuelLevel;

        // internal: visible solo dentro del proyecto/assembly
        internal string Vin { get; }

        // static: pertenece a la clase (contador global)
        public static int TotalCarsBuilt { get; private set; } = 0;

        // static readonly: constante "casi" (setea una vez)
        public static readonly int MaxFuel = 60;

        // Constructor: una interface no puede tener constructor; abstract sí.
        protected CarBase(string model, int fuelConsumptionPerKm, string vin, int initialFuel)
        {
            Model = model;
            FuelConsumptionPerKm = fuelConsumptionPerKm;
            Vin = vin;

            _fuelLevel = Math.Min(initialFuel, MaxFuel);

            TotalCarsBuilt++;
        }

        // virtual: comportamiento base que marcas pueden redefinir
        public virtual void Start()
        {
            Console.WriteLine($"{Model}: Engine started.");
        }

        // interfaz obliga: Drive y Refuel deben existir
        public void Drive(int km)
        {
            if (km <= 0) throw new ArgumentException("km must be > 0");

            var needed = km * FuelConsumptionPerKm;
            if (_fuelLevel < needed)
                throw new InvalidOperationException($"{Model}: Not enough fuel to drive {km}km");

            _fuelLevel -= needed;

            // hook para que la marca agregue comportamiento
            AfterDrive(km);

            Console.WriteLine($"{Model}: Drove {km}km. Fuel left: {_fuelLevel}L");
        }

        // protected virtual: hook para personalización en hijas
        protected virtual void AfterDrive(int km)
        {
            // por defecto no hace nada
        }

        // Overload: mismo nombre, distinta firma
        public void Refuel(int liters)
        {
            Refuel(liters, premium: false);
        }

        // Overload (2): ahora con parámetro extra
        public void Refuel(int liters, bool premium)
        {
            if (liters <= 0) throw new ArgumentException("liters must be > 0");

            _fuelLevel = Math.Min(_fuelLevel + liters, MaxFuel);
            Console.WriteLine($"{Model}: Refueled {liters}L (premium={premium}). Fuel: {_fuelLevel}L");
        }

        // abstract: obliga a cada marca a definir su "especialidad"
        public abstract string BrandSound();

        // virtual: se puede redefinir
        public virtual string Describe() =>
            $"{Model} (fuel={_fuelLevel}/{MaxFuel}, consumption={FuelConsumptionPerKm}L/km)";
    }

    // =========================
    // 4) Clases concretas (marcas)
    // =========================

    public sealed class Bmw : CarBase // sealed: no quiero heredar de BMW y hacer "BmwSuperHack"
    {
        // composición: el auto tiene ruedas
        private readonly Wheel[] _wheels;

        public Bmw(string vin)
            : base(model: "BMW 320i", fuelConsumptionPerKm: 1, vin: vin, initialFuel: 20)
        {
            _wheels = new[]
            {
                new Wheel(18), new Wheel(18), new Wheel(18), new Wheel(18)
            };
        }

        public override void Start()
        {
            base.Start();
            Console.WriteLine("BMW: Sport mode ready.");
        }

        protected override void AfterDrive(int km)
        {
            Console.WriteLine($"BMW: Traction control adjusted after {km}km.");
        }

        public override string BrandSound() => "BMW: Vroooom!";

        // sealed override: nadie puede volver a cambiar Describe si BMW no quiere
        public sealed override string Describe() =>
            base.Describe() + $" | wheels: {_wheels[0]}";
    }

    public class Toyota : CarBase
    {
        public Toyota(string vin)
            : base(model: "Toyota Corolla", fuelConsumptionPerKm: 1, vin: vin, initialFuel: 30)
        { }

        public override string BrandSound() => "Toyota: Brrrr!";

        // Ejemplo de "new" (OCULTA, NO overridea) — casi nunca recomendado:
        // public new void Start() { ... }
        // Si alguien te usa como CarBase, NO llamaría este Start.
    }

    // =========================
    // 5) Demo: ver diferencias en runtime
    // =========================

    public static class Program
    {
        // Example usage (commented out to avoid multiple entry points):
        // CarBase car1 = new Bmw("VIN-BMW-123");
        // CarBase car2 = new Toyota("VIN-TOY-999");
        // car1.Start();
        // Console.WriteLine(car1.BrandSound());
    }
}
