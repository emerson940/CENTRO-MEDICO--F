USE CentroMedico;
GO

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    /* =====================================================
       1. PACIENTES REALISTAS
       ===================================================== */

    INSERT INTO dbo.Pacientes
    (
        Documento,
        Nombre,
        FechaNacimiento,
        Telefono
    )
    SELECT
        Datos.Documento,
        Datos.Nombre,
        Datos.FechaNacimiento,
        Datos.Telefono
    FROM
    (
        VALUES
        (
            '73518426',
            N'Ana Lucía Rojas Díaz',
            DATEFROMPARTS(1996, 4, 18),
            N'987654321'
        ),
        (
            '46829715',
            N'Carlos Eduardo Mendoza Silva',
            DATEFROMPARTS(1984, 11, 2),
            N'956123789'
        ),
        (
            '81253647',
            N'Rosa Elena Vásquez Torres',
            DATEFROMPARTS(2002, 7, 29),
            N'978456123'
        ),
        (
            '60471835',
            N'José Luis Cabrera Pérez',
            DATEFROMPARTS(1977, 1, 15),
            N'965874231'
        ),
        (
            '75932468',
            N'Daniela Fernanda Salazar Ruiz',
            DATEFROMPARTS(1990, 9, 7),
            N'944782156'
        )
    ) AS Datos
    (
        Documento,
        Nombre,
        FechaNacimiento,
        Telefono
    )
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Pacientes P
        WHERE P.Documento = Datos.Documento
    );

    /* =====================================================
       2. MÉDICOS
       ===================================================== */

    INSERT INTO dbo.Medicos
    (
        Codigo,
        Nombre,
        Especialidad
    )
    SELECT
        Datos.Codigo,
        Datos.Nombre,
        Datos.Especialidad
    FROM
    (
        VALUES
        (
            'MED-GEN-01',
            N'Dra. Elena Vásquez Rojas',
            N'Medicina general'
        ),
        (
            'MED-PED-01',
            N'Dra. Ana Torres Medina',
            N'Pediatría'
        ),
        (
            'MED-CAR-01',
            N'Dr. Ricardo Salazar Paredes',
            N'Cardiología'
        ),
        (
            'MED-DER-01',
            N'Dra. Patricia Mendoza León',
            N'Dermatología'
        )
    ) AS Datos
    (
        Codigo,
        Nombre,
        Especialidad
    )
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Medicos M
        WHERE M.Codigo = Datos.Codigo
    );

    /* =====================================================
       3. INSUMOS MÉDICOS
       ===================================================== */

    INSERT INTO dbo.Insumos
    (
        Nombre,
        Unidad,
        Stock
    )
    SELECT
        Datos.Nombre,
        Datos.Unidad,
        Datos.Stock
    FROM
    (
        VALUES
        (
            N'Guantes de nitrilo',
            N'PAR',
            CAST(120.00 AS DECIMAL(12,2))
        ),
        (
            N'Gasas estériles',
            N'PAQUETE',
            CAST(80.00 AS DECIMAL(12,2))
        ),
        (
            N'Jeringas descartables 5 ml',
            N'UNIDAD',
            CAST(60.00 AS DECIMAL(12,2))
        ),
        (
            N'Alcohol medicinal 70%',
            N'FRASCO',
            CAST(25.00 AS DECIMAL(12,2))
        ),
        (
            N'Mascarillas quirúrgicas',
            N'UNIDAD',
            CAST(150.00 AS DECIMAL(12,2))
        ),
        (
            N'Venda elástica 4 pulgadas',
            N'ROLLO',
            CAST(20.00 AS DECIMAL(12,2))
        ),
        (
            N'Solución salina 0.9%',
            N'FRASCO',
            CAST(30.00 AS DECIMAL(12,2))
        ),
        (
            N'Catéter intravenoso N. 20',
            N'UNIDAD',
            CAST(0.00 AS DECIMAL(12,2))
        )
    ) AS Datos
    (
        Nombre,
        Unidad,
        Stock
    )
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Insumos I
        WHERE I.Nombre = Datos.Nombre
    );

    /* =====================================================
       4. CITAS PENDIENTES
       ===================================================== */

    INSERT INTO dbo.Citas
    (
        Codigo,
        PacienteID,
        MedicoID,
        Fecha,
        Tarifa,
        Estado
    )
    SELECT
        Datos.Codigo,
        P.PacienteID,
        M.MedicoID,
        Datos.Fecha,
        Datos.Tarifa,
        'PENDIENTE'
    FROM
    (
        VALUES
        (
            'CITA-20260923-001',
            '73518426',
            'MED-GEN-01',
            DATETIME2FROMPARTS(2026, 9, 23, 8, 30, 0, 0, 0),
            CAST(70.00 AS DECIMAL(12,2))
        ),
        (
            'CITA-20260923-002',
            '46829715',
            'MED-CAR-01',
            DATETIME2FROMPARTS(2026, 9, 23, 10, 0, 0, 0, 0),
            CAST(120.00 AS DECIMAL(12,2))
        ),
        (
            'CITA-20260923-003',
            '81253647',
            'MED-PED-01',
            DATETIME2FROMPARTS(2026, 9, 23, 11, 30, 0, 0, 0),
            CAST(80.00 AS DECIMAL(12,2))
        ),
        (
            'CITA-ROLLBACK-01',
            '60471835',
            'MED-GEN-01',
            DATETIME2FROMPARTS(2026, 9, 23, 14, 0, 0, 0, 0),
            CAST(90.00 AS DECIMAL(12,2))
        ),
        (
            'CITA-COMMIT-01',
            '75932468',
            'MED-DER-01',
            DATETIME2FROMPARTS(2026, 9, 23, 15, 30, 0, 0, 0),
            CAST(100.00 AS DECIMAL(12,2))
        )
    ) AS Datos
    (
        Codigo,
        DocumentoPaciente,
        CodigoMedico,
        Fecha,
        Tarifa
    )
    INNER JOIN dbo.Pacientes P
        ON P.Documento = Datos.DocumentoPaciente
    INNER JOIN dbo.Medicos M
        ON M.Codigo = Datos.CodigoMedico
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.Citas C
        WHERE C.Codigo = Datos.Codigo
    );

    COMMIT TRANSACTION;

    PRINT 'Datos realistas registrados correctamente.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

/* =========================================================
   VERIFICACIÓN
   ========================================================= */

SELECT
    PacienteID,
    Documento,
    Nombre,
    FechaNacimiento,
    Telefono
FROM dbo.Pacientes
ORDER BY PacienteID;

SELECT
    MedicoID,
    Codigo,
    Nombre,
    Especialidad
FROM dbo.Medicos
ORDER BY MedicoID;

SELECT
    InsumoID,
    Nombre,
    Unidad,
    Stock
FROM dbo.Insumos
ORDER BY InsumoID;

SELECT
    C.CitaID,
    C.Codigo,
    P.Nombre AS Paciente,
    M.Nombre AS Medico,
    M.Especialidad,
    C.Fecha,
    C.Tarifa,
    C.Estado
FROM dbo.Citas C
INNER JOIN dbo.Pacientes P
    ON P.PacienteID = C.PacienteID
INNER JOIN dbo.Medicos M
    ON M.MedicoID = C.MedicoID
ORDER BY C.CitaID;