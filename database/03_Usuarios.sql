

USE CentroMedico;
GO

IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Usuarios
    (
        UsuarioID INT IDENTITY(1,1) PRIMARY KEY,
        NombreUsuario NVARCHAR(40) NOT NULL UNIQUE,
        Clave NVARCHAR(100) NOT NULL,
        NombreCompleto NVARCHAR(100) NOT NULL,
        Rol NVARCHAR(20) NOT NULL,
        Activo BIT NOT NULL DEFAULT 1,

        CONSTRAINT CK_Usuarios_Rol
        CHECK (Rol IN ('ADMIN', 'MEDICO', 'RECEPCION'))
    );
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ValidarUsuario
    @NombreUsuario NVARCHAR(40),
    @Clave NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        UsuarioID,
        NombreUsuario,
        NombreCompleto,
        Rol,
        Activo
    FROM dbo.Usuarios
    WHERE NombreUsuario = @NombreUsuario
      AND Clave = @Clave
      AND Activo = 1;
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Usuarios
    WHERE NombreUsuario = N'admin'
)
BEGIN
    INSERT INTO dbo.Usuarios
    (
        NombreUsuario,
        Clave,
        NombreCompleto,
        Rol
    )
    VALUES
    (
        N'admin',
        N'admin123',
        N'Administrador del sistema',
        N'ADMIN'
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Usuarios
    WHERE NombreUsuario = N'medico'
)
BEGIN
    INSERT INTO dbo.Usuarios
    (
        NombreUsuario,
        Clave,
        NombreCompleto,
        Rol
    )
    VALUES
    (
        N'medico',
        N'med123',
        N'Médico de prueba',
        N'MEDICO'
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM dbo.Usuarios
    WHERE NombreUsuario = N'recepcion'
)
BEGIN
    INSERT INTO dbo.Usuarios
    (
        NombreUsuario,
        Clave,
        NombreCompleto,
        Rol
    )
    VALUES
    (
        N'recepcion',
        N'rec123',
        N'Recepcionista de prueba',
        N'RECEPCION'
    );
END;
GO

-------------
EXEC dbo.sp_ValidarUsuario
    @NombreUsuario = N'admin',
    @Clave = N'admin123';
GO
--------------
EXEC dbo.sp_ValidarUsuario
    @NombreUsuario = N'admin',
    @Clave = N'incorrecta';
GO