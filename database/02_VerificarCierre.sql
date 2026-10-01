USE CentroMedico;
GO
-- Solo consultas. Ejecutar antes y después del cierre o del fallo simulado.
DECLARE @CitaID INT =
(
    SELECT CitaID
    FROM dbo.Citas
    WHERE Codigo = 'DEMO-CITA-02'
);
SELECT CitaID, Codigo, Estado, Tarifa, 
RowVersion FROM dbo.Citas
WHERE CitaID = @CitaID;

SELECT * FROM dbo.Historial 
WHERE CitaID = @CitaID;

SELECT R.* FROM dbo.Recetas R 
INNER JOIN dbo.Historial H 
ON H.HistorialID = R.HistorialID
WHERE H.CitaID = @CitaID;

SELECT D.* FROM dbo.DetalleReceta D 
INNER JOIN dbo.Recetas R 
ON R.RecetaID = D.RecetaID
INNER JOIN dbo.Historial H
ON H.HistorialID = R.HistorialID 
WHERE H.CitaID = @CitaID;

SELECT F.* FROM dbo.Facturas F 
INNER JOIN dbo.Historial H
ON H.HistorialID = F.HistorialID
WHERE H.CitaID = @CitaID;

SELECT C.* FROM dbo.ConsumoInsumos C 
INNER JOIN dbo.Historial H 
ON H.HistorialID = C.HistorialID
WHERE H.CitaID = @CitaID;

SELECT InsumoID, Nombre, Stock 
FROM dbo.Insumos 
ORDER BY InsumoID;
GO

select*from dbo.Insumos
