USE CentroMedico;
GO

CREATE OR ALTER PROCEDURE dbo.sp_RegistrarPaciente
    @Documento VARCHAR(12),
    @Nombre NVARCHAR(100),
    @FechaNacimiento DATE,
    @Telefono NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @Documento = LTRIM(RTRIM(@Documento));
    SET @Nombre = LTRIM(RTRIM(@Nombre));
    SET @Telefono = NULLIF(LTRIM(RTRIM(@Telefono)), N'');

    IF @Documento = ''
        THROW 51020, 'Ingrese el documento del paciente.', 1;

    IF @Nombre = N''
        THROW 51021, 'Ingrese el nombre del paciente.', 1;

    IF @FechaNacimiento > CONVERT(DATE, GETDATE())
        THROW 51022, 'La fecha de nacimiento no puede ser futura.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Pacientes
        WHERE Documento = @Documento
    )
        THROW 51023, 'Ya existe un paciente con ese documento.', 1;

    INSERT INTO dbo.Pacientes
    (
        Documento,
        Nombre,
        FechaNacimiento,
        Telefono
    )
    VALUES
    (
        @Documento,
        @Nombre,
        @FechaNacimiento,
        @Telefono
    );

    SELECT CONVERT(INT, SCOPE_IDENTITY()) AS PacienteID;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ActualizarPaciente
    @PacienteID INT,
    @Documento VARCHAR(12),
    @Nombre NVARCHAR(100),
    @FechaNacimiento DATE,
    @Telefono NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SET @Documento = LTRIM(RTRIM(@Documento));
    SET @Nombre = LTRIM(RTRIM(@Nombre));
    SET @Telefono = NULLIF(LTRIM(RTRIM(@Telefono)), N'');

    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.Pacientes
        WHERE PacienteID = @PacienteID
    )
        THROW 51024, 'El paciente seleccionado no existe.', 1;

    IF @Documento = ''
        THROW 51020, 'Ingrese el documento del paciente.', 1;

    IF @Nombre = N''
        THROW 51021, 'Ingrese el nombre del paciente.', 1;

    IF @FechaNacimiento > CONVERT(DATE, GETDATE())
        THROW 51022, 'La fecha de nacimiento no puede ser futura.', 1;

    IF EXISTS
    (
        SELECT 1
        FROM dbo.Pacientes
        WHERE Documento = @Documento
          AND PacienteID <> @PacienteID
    )
        THROW 51023, 'Ya existe otro paciente con ese documento.', 1;

    UPDATE dbo.Pacientes
    SET Documento = @Documento,
        Nombre = @Nombre,
        FechaNacimiento = @FechaNacimiento,
        Telefono = @Telefono
    WHERE PacienteID = @PacienteID;
END;
GO