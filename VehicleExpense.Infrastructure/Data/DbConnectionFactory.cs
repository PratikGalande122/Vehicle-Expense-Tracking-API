using Microsoft.Data.Sqlite;
using System.Data;

namespace VehicleExpense.Infrastructure.Data;

/// <summary>
/// Creates and returns SQLite connections. Ensures the database and tables exist on startup.
/// </summary>
public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection() => new SqliteConnection(_connectionString);

    /// <summary>Creates all required tables if they do not already exist.</summary>
    public void InitializeDatabase()
    {
        using var conn = CreateConnection();
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            PRAGMA journal_mode=WAL;
            PRAGMA foreign_keys=ON;

            CREATE TABLE IF NOT EXISTS Users (
                Id                          INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName                    TEXT NOT NULL,
                Email                       TEXT NOT NULL UNIQUE,
                MobileNumber                TEXT,
                ProfileImageUrl             TEXT,
                DateOfBirth                 TEXT,
                Gender                      INTEGER,
                -- Address
                Address                     TEXT,
                City                        TEXT,
                State                       TEXT,
                Country                     TEXT,
                Pincode                     TEXT,
                -- Safety & medical
                BloodGroup                  INTEGER,
                MedicalConditions           TEXT,
                Allergies                   TEXT,
                -- Emergency contact
                EmergencyContactName        TEXT,
                EmergencyContactNumber      TEXT,
                EmergencyContactRelation    TEXT,
                -- Insurance
                InsuranceProvider           TEXT,
                InsurancePolicyNumber       TEXT,
                -- Preferred services
                PreferredMechanicName       TEXT,
                PreferredMechanicContact    TEXT,
                PreferredHospital           TEXT,
                PreferredHospitalContact    TEXT,
                -- License
                DrivingLicenseNumber        TEXT,
                LicenseExpiryDate           TEXT,
                -- Notes
                RiderNotes                  TEXT,
                -- Auth tokens
                OtpHash                     TEXT,
                OtpExpiry                   TEXT,
                RefreshToken                TEXT,
                RefreshTokenExpiry          TEXT,
                -- Audit
                CreatedAt                   TEXT NOT NULL,
                UpdatedAt                   TEXT,
                IsDeleted                   INTEGER NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS Vehicles (
                Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId              INTEGER NOT NULL,
                Name                TEXT NOT NULL,
                RegistrationNumber  TEXT NOT NULL,
                VehicleType         INTEGER NOT NULL,
                FuelType            INTEGER NOT NULL,
                Year                INTEGER NOT NULL,
                Brand               TEXT,
                Model               TEXT,
                CreatedAt           TEXT NOT NULL,
                UpdatedAt           TEXT,
                FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS FuelLogs (
                Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                VehicleId           INTEGER NOT NULL,
                UserId              INTEGER NOT NULL,
                LitresFilled        REAL NOT NULL,
                PricePerLitre       REAL NOT NULL,
                TotalCost           REAL NOT NULL,
                OdometerReading     REAL NOT NULL,
                FilledAt            TEXT NOT NULL,
                Notes               TEXT,
                CreatedAt           TEXT NOT NULL,
                UpdatedAt           TEXT,
                FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE CASCADE
            );


            CREATE TABLE IF NOT EXISTS Expenses (
                Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                VehicleId           INTEGER NOT NULL,
                UserId              INTEGER NOT NULL,
                ExpenseType         INTEGER NOT NULL,
                Amount              REAL NOT NULL,
                Description         TEXT,
                ExpenseDate         TEXT NOT NULL,
                Place               TEXT,
                Driver              TEXT,
                PaymentMethod       TEXT,
                Reason              TEXT,
                CreatedAt           TEXT NOT NULL,
                UpdatedAt           TEXT,
                FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE CASCADE
            );


            CREATE TABLE IF NOT EXISTS Reminders (
                Id                  INTEGER PRIMARY KEY AUTOINCREMENT,
                VehicleId           INTEGER NOT NULL,
                UserId              INTEGER NOT NULL,
                Title               TEXT NOT NULL,
                Description         TEXT,
                DueDate             TEXT NOT NULL,
                IsCompleted         INTEGER NOT NULL DEFAULT 0,
                CreatedAt           TEXT NOT NULL,
                UpdatedAt           TEXT,
                FOREIGN KEY (VehicleId) REFERENCES Vehicles(Id) ON DELETE CASCADE
            );
        ";
        cmd.ExecuteNonQuery();
    }

    /// <summary>Inserts demo seed data if the tables are empty.</summary>
    public void SeedDatabase()
    {
        using var conn = CreateConnection();
        conn.Open();

        using var checkCmd = conn.CreateCommand();
        checkCmd.CommandText = "SELECT COUNT(*) FROM Users;";
        var userCount = (long)(checkCmd.ExecuteScalar() ?? 0L);
        if (userCount > 0)
            return; // Already seeded

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            -- Demo User
            INSERT INTO Users (FullName, Email, MobileNumber, DateOfBirth, BloodGroup, EmergencyContactName, EmergencyContactNumber, EmergencyContactRelation, CreatedAt, IsDeleted)
            VALUES ('Demo User', 'demo@example.com', '9876543210', '1990-05-15', 6, 'John Doe', '9876543211', 'Father', datetime('now'), 0);

            -- ???????????????????????????????????????????????????????????????????
            -- VEHICLES (5 different types)
            -- ???????????????????????????????????????????????????????????????????
            
            -- Vehicle 1: Car - Toyota Corolla
            INSERT INTO Vehicles (UserId, Name, RegistrationNumber, VehicleType, FuelType, Year, Brand, Model, CreatedAt)
            VALUES (1, 'My Car', 'MH12AB1234', 2, 1, 2020, 'Toyota', 'Corolla', datetime('now', '-730 days'));

            -- Vehicle 2: Bike - Honda CB350
            INSERT INTO Vehicles (UserId, Name, RegistrationNumber, VehicleType, FuelType, Year, Brand, Model, CreatedAt)
            VALUES (1, 'Work Bike', 'MH14XY5678', 1, 1, 2022, 'Honda', 'CB350', datetime('now', '-365 days'));

            -- Vehicle 3: Scooter - Activa Electric
            INSERT INTO Vehicles (UserId, Name, RegistrationNumber, VehicleType, FuelType, Year, Brand, Model, CreatedAt)
            VALUES (1, 'City Scooter', 'MH01CD9876', 3, 3, 2023, 'Honda', 'Activa Electric', datetime('now', '-180 days'));

            -- Vehicle 4: Truck - Tata Ace
            INSERT INTO Vehicles (UserId, Name, RegistrationNumber, VehicleType, FuelType, Year, Brand, Model, CreatedAt)
            VALUES (1, 'Delivery Truck', 'MH20TR4455', 4, 2, 2019, 'Tata', 'Ace', datetime('now', '-1095 days'));

            -- Vehicle 5: Hybrid SUV
            INSERT INTO Vehicles (UserId, Name, RegistrationNumber, VehicleType, FuelType, Year, Brand, Model, CreatedAt)
            VALUES (1, 'Family SUV', 'MH02EF7788', 2, 5, 2024, 'Toyota', 'Hyryder', datetime('now', '-90 days'));

            -- ???????????????????????????????????????????????????????????????????
            -- FUEL LOGS (multiple entries per vehicle)
            -- ???????????????????????????????????????????????????????????????????

            -- Car (Vehicle 1) - 6 fuel logs
            INSERT INTO FuelLogs (VehicleId, UserId, LitresFilled, PricePerLitre, TotalCost, OdometerReading, FilledAt, Notes, CreatedAt)
            VALUES 
                (1, 1, 45.0, 102.5, 4612.5, 5000, datetime('now', '-180 days'), 'Full tank', datetime('now', '-180 days')),
                (1, 1, 40.0, 103.0, 4120.0, 5500, datetime('now', '-150 days'), 'Highway trip', datetime('now', '-150 days')),
                (1, 1, 42.5, 104.2, 4428.5, 6200, datetime('now', '-120 days'), NULL, datetime('now', '-120 days')),
                (1, 1, 38.0, 105.0, 3990.0, 6800, datetime('now', '-90 days'), '3/4 tank', datetime('now', '-90 days')),
                (1, 1, 44.0, 106.5, 4686.0, 7450, datetime('now', '-60 days'), 'Full tank', datetime('now', '-60 days')),
                (1, 1, 41.0, 107.0, 4387.0, 8100, datetime('now', '-30 days'), NULL, datetime('now', '-30 days'));

            -- Bike (Vehicle 2) - 8 fuel logs
            INSERT INTO FuelLogs (VehicleId, UserId, LitresFilled, PricePerLitre, TotalCost, OdometerReading, FilledAt, Notes, CreatedAt)
            VALUES 
                (2, 1, 12.0, 102.0, 1224.0, 2000, datetime('now', '-300 days'), 'First fill', datetime('now', '-300 days')),
                (2, 1, 10.5, 103.5, 1086.75, 2450, datetime('now', '-270 days'), NULL, datetime('now', '-270 days')),
                (2, 1, 11.0, 104.0, 1144.0, 2900, datetime('now', '-240 days'), NULL, datetime('now', '-240 days')),
                (2, 1, 12.5, 105.0, 1312.5, 3380, datetime('now', '-210 days'), 'Full tank', datetime('now', '-210 days')),
                (2, 1, 10.0, 106.0, 1060.0, 3820, datetime('now', '-180 days'), NULL, datetime('now', '-180 days')),
                (2, 1, 11.5, 107.5, 1236.25, 4300, datetime('now', '-150 days'), NULL, datetime('now', '-150 days')),
                (2, 1, 12.0, 108.0, 1296.0, 4780, datetime('now', '-120 days'), NULL, datetime('now', '-120 days')),
                (2, 1, 10.5, 109.0, 1144.5, 5200, datetime('now', '-90 days'), NULL, datetime('now', '-90 days'));

            -- Scooter (Vehicle 3) - Electric, no fuel logs (charging only)

            -- Truck (Vehicle 4) - 5 fuel logs
            INSERT INTO FuelLogs (VehicleId, UserId, LitresFilled, PricePerLitre, TotalCost, OdometerReading, FilledAt, Notes, CreatedAt)
            VALUES 
                (4, 1, 55.0, 95.0, 5225.0, 15000, datetime('now', '-150 days'), 'Diesel full tank', datetime('now', '-150 days')),
                (4, 1, 50.0, 96.5, 4825.0, 15800, datetime('now', '-120 days'), NULL, datetime('now', '-120 days')),
                (4, 1, 52.0, 97.0, 5044.0, 16650, datetime('now', '-90 days'), NULL, datetime('now', '-90 days')),
                (4, 1, 54.0, 98.5, 5319.0, 17500, datetime('now', '-60 days'), NULL, datetime('now', '-60 days')),
                (4, 1, 53.0, 99.0, 5247.0, 18350, datetime('now', '-30 days'), NULL, datetime('now', '-30 days'));

            -- Hybrid SUV (Vehicle 5) - 3 fuel logs
            INSERT INTO FuelLogs (VehicleId, UserId, LitresFilled, PricePerLitre, TotalCost, OdometerReading, FilledAt, Notes, CreatedAt)
            VALUES 
                (5, 1, 35.0, 105.0, 3675.0, 1000, datetime('now', '-60 days'), 'Hybrid fuel efficient', datetime('now', '-60 days')),
                (5, 1, 32.0, 106.0, 3392.0, 2200, datetime('now', '-30 days'), NULL, datetime('now', '-30 days')),
                (5, 1, 33.5, 107.0, 3584.5, 3450, datetime('now', '-10 days'), NULL, datetime('now', '-10 days'));

            -- ???????????????????????????????????????????????????????????????????
            -- EXPENSES (various types per vehicle)
            -- ???????????????????????????????????????????????????????????????????

            -- Car (Vehicle 1) expenses
            INSERT INTO Expenses (VehicleId, UserId, ExpenseType, Amount, Description, ExpenseDate, CreatedAt)
            VALUES 
                (1, 1, 2, 3500.0, 'Engine oil change + filter', datetime('now', '-160 days'), datetime('now', '-160 days')),
                (1, 1, 3, 12500.0, 'Annual comprehensive insurance', datetime('now', '-140 days'), datetime('now', '-140 days')),
                (1, 1, 6, 8500.0, 'Front brake pad replacement', datetime('now', '-100 days'), datetime('now', '-100 days')),
                (1, 1, 8, 500.0, 'Car wash premium', datetime('now', '-80 days'), datetime('now', '-80 days')),
                (1, 1, 4, 200.0, 'Mall parking', datetime('now', '-70 days'), datetime('now', '-70 days')),
                (1, 1, 5, 450.0, 'Highway toll', datetime('now', '-65 days'), datetime('now', '-65 days')),
                (1, 1, 7, 2500.0, 'Dashboard camera installation', datetime('now', '-50 days'), datetime('now', '-50 days')),
                (1, 1, 2, 4500.0, 'Full service + alignment', datetime('now', '-20 days'), datetime('now', '-20 days'));

            -- Bike (Vehicle 2) expenses
            INSERT INTO Expenses (VehicleId, UserId, ExpenseType, Amount, Description, ExpenseDate, CreatedAt)
            VALUES 
                (2, 1, 2, 1200.0, 'Chain cleaning + lube', datetime('now', '-250 days'), datetime('now', '-250 days')),
                (2, 1, 3, 4500.0, 'Third party insurance', datetime('now', '-230 days'), datetime('now', '-230 days')),
                (2, 1, 6, 3500.0, 'Rear tyre replacement', datetime('now', '-180 days'), datetime('now', '-180 days')),
                (2, 1, 8, 200.0, 'Bike wash', datetime('now', '-150 days'), datetime('now', '-150 days')),
                (2, 1, 7, 800.0, 'Mobile holder', datetime('now', '-130 days'), datetime('now', '-130 days')),
                (2, 1, 2, 1500.0, 'Engine oil + air filter', datetime('now', '-90 days'), datetime('now', '-90 days')),
                (2, 1, 4, 50.0, 'Parking fee', datetime('now', '-60 days'), datetime('now', '-60 days'));

            -- Scooter (Vehicle 3) expenses
            INSERT INTO Expenses (VehicleId, UserId, ExpenseType, Amount, Description, ExpenseDate, CreatedAt)
            VALUES 
                (3, 1, 3, 3500.0, 'Insurance renewal', datetime('now', '-150 days'), datetime('now', '-150 days')),
                (3, 1, 9, 500.0, 'Charging station fee', datetime('now', '-120 days'), datetime('now', '-120 days')),
                (3, 1, 2, 800.0, 'Battery health checkup', datetime('now', '-90 days'), datetime('now', '-90 days')),
                (3, 1, 7, 1200.0, 'Seat cover + floor mat', datetime('now', '-60 days'), datetime('now', '-60 days')),
                (3, 1, 8, 150.0, 'Scooter wash', datetime('now', '-30 days'), datetime('now', '-30 days'));

            -- Truck (Vehicle 4) expenses
            INSERT INTO Expenses (VehicleId, UserId, ExpenseType, Amount, Description, ExpenseDate, CreatedAt)
            VALUES 
                (4, 1, 3, 18000.0, 'Commercial vehicle insurance', datetime('now', '-140 days'), datetime('now', '-140 days')),
                (4, 1, 2, 6500.0, 'Major service + oil change', datetime('now', '-120 days'), datetime('now', '-120 days')),
                (4, 1, 6, 15000.0, 'All 4 tyres replacement', datetime('now', '-90 days'), datetime('now', '-90 days')),
                (4, 1, 5, 2500.0, 'Interstate toll charges', datetime('now', '-70 days'), datetime('now', '-70 days')),
                (4, 1, 4, 800.0, 'Loading area parking', datetime('now', '-50 days'), datetime('now', '-50 days')),
                (4, 1, 6, 5500.0, 'Clutch plate replacement', datetime('now', '-30 days'), datetime('now', '-30 days'));

            -- Hybrid SUV (Vehicle 5) expenses
            INSERT INTO Expenses (VehicleId, UserId, ExpenseType, Amount, Description, ExpenseDate, CreatedAt)
            VALUES 
                (5, 1, 3, 22000.0, 'Zero depreciation insurance', datetime('now', '-70 days'), datetime('now', '-70 days')),
                (5, 1, 2, 2500.0, 'First free service', datetime('now', '-50 days'), datetime('now', '-50 days')),
                (5, 1, 7, 8500.0, 'Roof rails + step board', datetime('now', '-40 days'), datetime('now', '-40 days')),
                (5, 1, 8, 800.0, 'Premium wash + coating', datetime('now', '-20 days'), datetime('now', '-20 days')),
                (5, 1, 4, 300.0, 'Airport parking', datetime('now', '-10 days'), datetime('now', '-10 days'));

            -- ???????????????????????????????????????????????????????????????????
            -- REMINDERS (upcoming maintenance per vehicle)
            -- ???????????????????????????????????????????????????????????????????

            -- Car (Vehicle 1) reminders
            INSERT INTO Reminders (VehicleId, UserId, Title, Description, DueDate, IsCompleted, CreatedAt)
            VALUES 
                (1, 1, 'Tyre rotation', 'Rotate all 4 tyres', datetime('now', '+15 days'), 0, datetime('now')),
                (1, 1, 'Insurance renewal', 'Comprehensive insurance due', datetime('now', '+45 days'), 0, datetime('now')),
                (1, 1, 'PUC check', 'Pollution Under Control certificate', datetime('now', '+60 days'), 0, datetime('now'));

            -- Bike (Vehicle 2) reminders
            INSERT INTO Reminders (VehicleId, UserId, Title, Description, DueDate, IsCompleted, CreatedAt)
            VALUES 
                (2, 1, 'Chain maintenance', 'Clean and lubricate chain', datetime('now', '+10 days'), 0, datetime('now')),
                (2, 1, 'Insurance due', 'Third party insurance renewal', datetime('now', '+90 days'), 0, datetime('now'));

            -- Scooter (Vehicle 3) reminders
            INSERT INTO Reminders (VehicleId, UserId, Title, Description, DueDate, IsCompleted, CreatedAt)
            VALUES 
                (3, 1, 'Battery checkup', 'Battery health and charging system check', datetime('now', '+20 days'), 0, datetime('now')),
                (3, 1, 'Software update', 'Visit service center for firmware update', datetime('now', '+30 days'), 0, datetime('now'));

            -- Truck (Vehicle 4) reminders
            INSERT INTO Reminders (VehicleId, UserId, Title, Description, DueDate, IsCompleted, CreatedAt)
            VALUES 
                (4, 1, 'Major service due', 'Engine oil + all filters', datetime('now', '+25 days'), 0, datetime('now')),
                (4, 1, 'Fitness certificate', 'Commercial vehicle fitness renewal', datetime('now', '+120 days'), 0, datetime('now')),
                (4, 1, 'Permit renewal', 'State permit renewal', datetime('now', '+150 days'), 0, datetime('now'));

            -- Hybrid SUV (Vehicle 5) reminders
            INSERT INTO Reminders (VehicleId, UserId, Title, Description, DueDate, IsCompleted, CreatedAt)
            VALUES 
                (5, 1, 'Second service', 'Due at 10000 km', datetime('now', '+40 days'), 0, datetime('now')),
                (5, 1, 'Hybrid battery check', 'Hybrid system health checkup', datetime('now', '+180 days'), 0, datetime('now'));
        ";
        cmd.ExecuteNonQuery();
    }
}
