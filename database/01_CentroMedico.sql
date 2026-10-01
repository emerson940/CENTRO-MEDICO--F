-- Centro Médico: esquema y datos ficticios para la evaluación.
-- Ejecutar en SQL Server Management Studio. No elimina una base existente.
IF DB_ID(N'CentroMedico') IS NULL
    EXEC(N'CREATE DATABASE CentroMedico');
GO
USE CentroMedico;
GO
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.Pacientes', N'U') IS NULL
    CREATE TABLE dbo.Pacientes
    (
        PacienteID INT IDENTITY PRIMARY KEY,
        Documento VARCHAR(12) NOT NULL UNIQUE,
        Nombre NVARCHAR(100) NOT NULL,
        FechaNacimiento DATE NOT NULL,
        Telefono NVARCHAR(20) NULL
    );

    IF OBJECT_ID(N'dbo.Medicos', N'U') IS NULL
    CREATE TABLE dbo.Medicos
    (
        MedicoID INT IDENTITY PRIMARY KEY,
        Codigo VARCHAR(12) NOT NULL UNIQUE,
        Nombre NVARCHAR(100) NOT NULL,
        Especialidad NVARCHAR(80) NOT NULL
    );

    IF OBJECT_ID(N'dbo.Citas', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Citas
        (
            CitaID INT IDENTITY PRIMARY KEY,
            Codigo VARCHAR(25) NOT NULL UNIQUE,
            PacienteID INT NOT NULL REFERENCES dbo.Pacientes(PacienteID),
            MedicoID INT NOT NULL REFERENCES dbo.Medicos(MedicoID),
            Fecha DATETIME2(0) NOT NULL,
            Tarifa DECIMAL(12,2) NOT NULL CHECK (Tarifa > 0),
            Estado VARCHAR(12) NOT NULL DEFAULT 'PENDIENTE'
                CHECK (Estado IN ('PENDIENTE', 'CERRADA', 'CANCELADA')),
            RowVersion ROWVERSION NOT NULL
        );
        CREATE UNIQUE INDEX UX_Citas_Medico_Fecha
            ON dbo.Citas(MedicoID, Fecha) WHERE Estado <> 'CANCELADA';
        CREATE INDEX IX_Citas_Paciente ON dbo.Citas(PacienteID, Fecha);
    END;

    IF OBJECT_ID(N'dbo.Historial', N'U') IS NULL
    CREATE TABLE dbo.Historial
    (
        HistorialID INT IDENTITY PRIMARY KEY,
        CitaID INT NOT NULL UNIQUE REFERENCES dbo.Citas(CitaID),
        Fecha DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
        Diagnostico NVARCHAR(500) NOT NULL CHECK (LEN(LTRIM(RTRIM(Diagnostico))) > 0),
        Observaciones NVARCHAR(1000) NULL
    );

    IF OBJECT_ID(N'dbo.Recetas', N'U') IS NULL
    CREATE TABLE dbo.Recetas
    (
        RecetaID INT IDENTITY PRIMARY KEY,
        HistorialID INT NOT NULL UNIQUE REFERENCES dbo.Historial(HistorialID),
        Fecha DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
        Indicaciones NVARCHAR(1000) NOT NULL
    );

    IF OBJECT_ID(N'dbo.DetalleReceta', N'U') IS NULL
    CREATE TABLE dbo.DetalleReceta
    (
        DetalleID INT IDENTITY PRIMARY KEY,
        RecetaID INT NOT NULL REFERENCES dbo.Recetas(RecetaID),
        Medicamento NVARCHAR(100) NOT NULL,
        Dosis NVARCHAR(100) NOT NULL,
        Frecuencia NVARCHAR(100) NOT NULL,
        Duracion NVARCHAR(100) NOT NULL
    );

    -- Facturas representa la orden de cobro de la consulta, todavía pendiente de pago.
    -- El importe de este alcance es la tarifa registrada en la cita.
    IF OBJECT_ID(N'dbo.Facturas', N'U') IS NULL
    CREATE TABLE dbo.Facturas
    (
        FacturaID INT IDENTITY PRIMARY KEY,
        HistorialID INT NOT NULL UNIQUE REFERENCES dbo.Historial(HistorialID),
        Fecha DATETIME2(0) NOT NULL DEFAULT SYSDATETIME(),
        Total DECIMAL(12,2) NOT NULL CHECK (Total > 0),
        Estado VARCHAR(12) NOT NULL DEFAULT 'PENDIENTE'
            CHECK (Estado IN ('PENDIENTE', 'PAGADA', 'ANULADA'))
    );

    IF OBJECT_ID(N'dbo.Insumos', N'U') IS NULL
    CREATE TABLE dbo.Insumos
    (
        InsumoID INT IDENTITY PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL UNIQUE,
        Unidad NVARCHAR(20) NOT NULL,
        Stock DECIMAL(12,2) NOT NULL CHECK (Stock >= 0),
        RowVersion ROWVERSION NOT NULL
    );

    IF OBJECT_ID(N'dbo.ConsumoInsumos', N'U') IS NULL
    CREATE TABLE dbo.ConsumoInsumos
    (
        HistorialID INT NOT NULL REFERENCES dbo.Historial(HistorialID),
        InsumoID INT NOT NULL REFERENCES dbo.Insumos(InsumoID),
        Cantidad DECIMAL(12,2) NOT NULL CHECK (Cantidad > 0),
        CONSTRAINT PK_ConsumoInsumos PRIMARY KEY (HistorialID, InsumoID)
    );

    -- El seed puede repetirse: no reinicia existencias ni reabre citas cerradas.
    IF NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE Documento = 'DEMO0001')
        INSERT INTO dbo.Pacientes(Documento, Nombre, FechaNacimiento, Telefono)
        VALUES ('DEMO0001', N'Paciente de prueba Uno', '20000115', NULL);
    IF NOT EXISTS (SELECT 1 FROM dbo.Pacientes WHERE Documento = 'DEMO0002')
        INSERT INTO dbo.Pacientes(Documento, Nombre, FechaNacimiento, Telefono)
        VALUES ('DEMO0002', N'Paciente de prueba Dos', '19950920', NULL);

    IF NOT EXISTS (SELECT 1 FROM dbo.Medicos WHERE Codigo = 'MED-DEMO-01')
        INSERT INTO dbo.Medicos(Codigo, Nombre, Especialidad)
        VALUES ('MED-DEMO-01', N'Médico de prueba', N'Medicina general');

    DECLARE @Paciente1 INT = (SELECT PacienteID FROM dbo.Pacientes WHERE Documento = 'DEMO0001');
    DECLARE @Paciente2 INT = (SELECT PacienteID FROM dbo.Pacientes WHERE Documento = 'DEMO0002');
    DECLARE @Medico INT = (SELECT MedicoID FROM dbo.Medicos WHERE Codigo = 'MED-DEMO-01');
    DECLARE @Dia DATETIME2(0) = CONVERT(DATE, GETDATE());

    IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE Codigo = 'DEMO-CITA-01')
        INSERT INTO dbo.Citas(Codigo, PacienteID, MedicoID, Fecha, Tarifa)
        VALUES ('DEMO-CITA-01', @Paciente1, @Medico, DATEADD(HOUR, 9, @Dia), 60.00);
    IF NOT EXISTS (SELECT 1 FROM dbo.Citas WHERE Codigo = 'DEMO-CITA-02')
        INSERT INTO dbo.Citas(Codigo, PacienteID, MedicoID, Fecha, Tarifa)
        VALUES ('DEMO-CITA-02', @Paciente2, @Medico, DATEADD(HOUR, 10, @Dia), 80.00);

    IF NOT EXISTS (SELECT 1 FROM dbo.Insumos WHERE Nombre = N'Guantes de prueba')
        INSERT INTO dbo.Insumos(Nombre, Unidad, Stock) VALUES (N'Guantes de prueba', N'par', 100.00);
    IF NOT EXISTS (SELECT 1 FROM dbo.Insumos WHERE Nombre = N'Gasas de prueba')
        INSERT INTO dbo.Insumos(Nombre, Unidad, Stock) VALUES (N'Gasas de prueba', N'unidad', 50.00);
    IF NOT EXISTS (SELECT 1 FROM dbo.Insumos WHERE Nombre = N'Insumo agotado - prueba Rollback')
        INSERT INTO dbo.Insumos(Nombre, Unidad, Stock)
        VALUES (N'Insumo agotado - prueba Rollback', N'unidad', 0.00);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
