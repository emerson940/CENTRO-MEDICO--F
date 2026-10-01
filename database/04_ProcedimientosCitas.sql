USE CentroMedico;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ListarPacientes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT PacienteID,
           Documento,
           Nombre,
           FechaNacimiento,
           Telefono
    FROM dbo.Pacientes
    ORDER BY Nombre;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ListarMedicos
AS
BEGIN
    SET NOCOUNT ON;

    SELECT MedicoID,
           Codigo,
           Nombre,
           Especialidad
    FROM dbo.Medicos
    ORDER BY Nombre;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ListarCitas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT C.CitaID,
           C.Codigo,
           C.Fecha,
           C.Tarifa,
           C.Estado,
           C.RowVersion,
           P.PacienteID,
           P.Documento,
           P.Nombre AS Paciente,
           M.MedicoID,
           M.Codigo AS CodigoMedico,
           M.Nombre AS Medico,
           M.Especialidad
    FROM dbo.Citas C
    INNER JOIN dbo.Pacientes P
        ON P.PacienteID = C.PacienteID
    INNER JOIN dbo.Medicos M
        ON M.MedicoID = C.MedicoID
    ORDER BY C.Fecha DESC;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarCita
    @Codigo VARCHAR(25),
    @PacienteID INT,
    @MedicoID INT,
    @Fecha DATETIME2(0),
    @Tarifa DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;

    IF LEN(LTRIM(RTRIM(@Codigo))) = 0
        THROW 51001, 'Ingrese el código de la cita.', 1;

    IF @Tarifa <= 0
        THROW 51002, 'La tarifa debe ser mayor que cero.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Pacientes
        WHERE PacienteID = @PacienteID
    )
        THROW 51003, 'El paciente no existe.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Medicos
        WHERE MedicoID = @MedicoID
    )
        THROW 51004, 'El médico no existe.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Citas
        WHERE Codigo = @Codigo
    )
        THROW 51005, 'El código de la cita ya existe.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Citas
        WHERE MedicoID = @MedicoID
          AND Fecha = @Fecha
          AND Estado <> 'CANCELADA'
    )
        THROW 51006, 'El médico ya tiene una cita en esa fecha y hora.', 1;

    INSERT INTO dbo.Citas
    (
        Codigo,
        PacienteID,
        MedicoID,
        Fecha,
        Tarifa,
        Estado
    )
    VALUES
    (
        @Codigo,
        @PacienteID,
        @MedicoID,
        @Fecha,
        @Tarifa,
        'PENDIENTE'
    );

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS CitaID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ReprogramarCita
    @CitaID INT,
    @MedicoID INT,
    @Fecha DATETIME2(0),
    @Tarifa DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Citas
        WHERE CitaID = @CitaID
          AND Estado = 'PENDIENTE'
    )
        THROW 51007, 'La cita no existe o ya no está pendiente.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Medicos
        WHERE MedicoID = @MedicoID
    )
        THROW 51008, 'El médico no existe.', 1;

    IF @Tarifa <= 0
        THROW 51009, 'La tarifa debe ser mayor que cero.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Citas
        WHERE MedicoID = @MedicoID
          AND Fecha = @Fecha
          AND Estado <> 'CANCELADA'
          AND CitaID <> @CitaID
    )
        THROW 51010, 'El médico ya tiene una cita en esa fecha y hora.', 1;

    UPDATE dbo.Citas
    SET MedicoID = @MedicoID,
        Fecha = @Fecha,
        Tarifa = @Tarifa
    WHERE CitaID = @CitaID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_CancelarCita
    @CitaID INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Citas
    SET Estado = 'CANCELADA'
    WHERE CitaID = @CitaID
      AND Estado = 'PENDIENTE';

    IF @@ROWCOUNT = 0
        THROW 51011, 'La cita no existe o ya no está pendiente.', 1;
END;
GO

-----------------------------
EXEC dbo.sp_ListarPacientes;
EXEC dbo.sp_ListarMedicos;
EXEC dbo.sp_ListarCitas;