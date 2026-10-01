USE CentroMedico;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarMedico
    @Codigo varchar(12),
    @Nombre nvarchar(100),
    @Especialidad nvarchar(80)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Codigo = LTRIM(RTRIM(@Codigo));
    SET @Nombre = LTRIM(RTRIM(@Nombre));
    SET @Especialidad = LTRIM(RTRIM(@Especialidad));

    IF NULLIF(@Codigo, '') IS NULL
        THROW 51030, 'El código del médico es obligatorio.', 1;

    IF NULLIF(@Nombre, '') IS NULL
        THROW 51031, 'El nombre del médico es obligatorio.', 1;

    IF NULLIF(@Especialidad, '') IS NULL
        THROW 51032, 'La especialidad es obligatoria.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Medicos
        WHERE Codigo = @Codigo
    )
        THROW 51033, 'Ya existe un médico con ese código.', 1;

    INSERT INTO dbo.Medicos
    (
        Codigo,
        Nombre,
        Especialidad
    )
    VALUES
    (
        @Codigo,
        @Nombre,
        @Especialidad
    );

    SELECT CAST(SCOPE_IDENTITY() AS int) AS MedicoID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ActualizarMedico
    @MedicoID int,
    @Codigo varchar(12),
    @Nombre nvarchar(100),
    @Especialidad nvarchar(80)
AS
BEGIN
    SET NOCOUNT ON;

    IF @MedicoID <= 0
        THROW 51034, 'Seleccione un médico válido.', 1;

    SET @Codigo = LTRIM(RTRIM(@Codigo));
    SET @Nombre = LTRIM(RTRIM(@Nombre));
    SET @Especialidad = LTRIM(RTRIM(@Especialidad));

    IF NULLIF(@Codigo, '') IS NULL
        THROW 51035, 'El código del médico es obligatorio.', 1;

    IF NULLIF(@Nombre, '') IS NULL
        THROW 51036, 'El nombre del médico es obligatorio.', 1;

    IF NULLIF(@Especialidad, '') IS NULL
        THROW 51037, 'La especialidad es obligatoria.', 1;

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Medicos
        WHERE MedicoID = @MedicoID
    )
        THROW 51038, 'El médico seleccionado no existe.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Medicos
        WHERE Codigo = @Codigo
          AND MedicoID <> @MedicoID
    )
        THROW 51039, 'Ya existe otro médico con ese código.', 1;

    UPDATE dbo.Medicos
    SET Codigo = @Codigo,
        Nombre = @Nombre,
        Especialidad = @Especialidad
    WHERE MedicoID = @MedicoID;
END;
GO