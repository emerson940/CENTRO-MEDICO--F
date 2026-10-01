USE CentroMedico;
GO
DECLARE @CitaID int = 9; -- Usar 5 para consultar la prueba de Commit.
 
SELECT CitaID, Codigo, Fecha, Tarifa, Estado
FROM dbo.Citas
WHERE CitaID = @CitaID;
 
SELECT
    (SELECT COUNT(*) FROM dbo.Historial
     WHERE CitaID = @CitaID) AS Historiales,
    (SELECT COUNT(*) FROM dbo.Recetas R
     INNER JOIN dbo.Historial H ON H.HistorialID = R.HistorialID
     WHERE H.CitaID = @CitaID) AS Recetas,
    (SELECT COUNT(*) FROM dbo.DetalleReceta D
     INNER JOIN dbo.Recetas R ON R.RecetaID = D.RecetaID
     INNER JOIN dbo.Historial H ON H.HistorialID = R.HistorialID
     WHERE H.CitaID = @CitaID) AS DetallesReceta,
    (SELECT COUNT(*) FROM dbo.Facturas F
     INNER JOIN dbo.Historial H ON H.HistorialID = F.HistorialID
     WHERE H.CitaID = @CitaID) AS Facturas,
    (SELECT COUNT(*) FROM dbo.ConsumoInsumos C
     INNER JOIN dbo.Historial H ON H.HistorialID = C.HistorialID
     WHERE H.CitaID = @CitaID) AS Consumos;
 
SELECT InsumoID, Nombre, Unidad, Stock
FROM dbo.Insumos
WHERE InsumoID IN (4, 5)
ORDER BY InsumoID;
 
SELECT F.FacturaID, F.Fecha, F.Total, F.Estado
FROM dbo.Facturas F
INNER JOIN dbo.Historial H ON H.HistorialID = F.HistorialID
WHERE H.CitaID = @CitaID;
