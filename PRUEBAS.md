# Prueba del cierre de consulta

Ejecutar sobre la base de demostración creada por `01_CentroMedico.sql`.

## 1. Reversión después de escrituras parciales

1. Ejecutar `02_VerificarCierre.sql` y guardar una captura del estado inicial de `DEMO-CITA-01` y los stocks.
2. En la aplicación, seleccionar `DEMO-CITA-01`.
3. Escribir `Diagnóstico de prueba académica` e `Indicaciones de prueba`.
4. Agregar un medicamento llamado `Medicamento ficticio`; usar `Dato de prueba` en dosis, frecuencia y duración.
5. Agregar `Guantes de prueba`, cantidad `1`.
6. Agregar `Insumo agotado - prueba Rollback`, cantidad `1`.
7. Cerrar la consulta. Debe mostrarse el error de stock insuficiente.
8. Volver a ejecutar `02_VerificarCierre.sql`.

Resultado esperado: cita PENDIENTE, ningún historial/receta/factura/consumo para esa cita y stocks iguales a los iniciales. El ID del insumo agotado se inserta después del de guantes en una base nueva; el descuento previo de guantes también debe revertirse.

Para mostrar la transacción durante la exposición, colocar un punto de interrupción antes del `throw` por stock insuficiente y otro en `tx.Rollback()`. Al llegar al primero, historial, receta y orden ya se intentaron registrar dentro de la transacción aún no confirmada.

Los contadores IDENTITY pueden avanzar aunque se reviertan las filas; los saltos de ID no significan que quedaron registros parciales.

## 2. Confirmación completa

1. En el mismo formulario, seleccionar el insumo agotado en la lista de consumos y presionar **Quitar seleccionado**.
2. Mantener el medicamento y `Guantes de prueba`, cantidad `1`.
3. Cerrar la consulta y guardar una captura del mensaje de orden generada.
4. Consultar Historial, Recetas y Facturación.
5. Ejecutar `02_VerificarCierre.sql`.

Resultado esperado: cita CERRADA, un historial, una receta con su detalle, una orden PENDIENTE por S/ 60.00 y un consumo. El stock de guantes disminuye exactamente una unidad.

## 3. Doble cierre y versión antigua

1. Antes de cerrar `DEMO-CITA-02`, abrir dos instancias de la aplicación y cargar la misma cita en ambas.
2. Completar los datos y cerrar en la primera.
3. Intentar cerrar desde la segunda, conservando la versión que cargó inicialmente.

Resultado esperado: la segunda operación se rechaza, con una sola orden/historial para esa cita y un único descuento. La comprobación de RowVersion y el bloqueo de la cita se hacen dentro de la transacción.

## 4. Existencias insuficientes sin insumo agotado

En otra cita PENDIENTE, agregar una cantidad superior al stock de un insumo y cerrar. Se espera el mismo Rollback completo, sin existencias negativas.

Reejecutar el seed no reabre citas ni repone stock: conserva los cambios confirmados.
