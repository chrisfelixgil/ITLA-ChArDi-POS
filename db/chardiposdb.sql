
CREATE DATABASE chardi_pos_db;
USE chardi_pos_db;

CREATE TABLE productos (
    id INT NOT NULL AUTO_INCREMENT,
    codigo VARCHAR(50) NOT NULL,
    nombre VARCHAR(150) NOT NULL,
    descripcion VARCHAR(255) NULL,
    precio DECIMAL(10,2) NOT NULL,
    stock INT NOT NULL,
    activo TINYINT(1) NOT NULL DEFAULT 1,
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_productos_codigo (codigo),
    CONSTRAINT chk_productos_precio CHECK (precio >= 0),
    CONSTRAINT chk_productos_stock CHECK (stock >= 0)
) ENGINE=InnoDB;

-- Índices
CREATE INDEX idx_productos_nombre ON productos(nombre);
CREATE INDEX idx_productos_activo ON productos(activo);

CREATE TABLE facturas (
    id INT NOT NULL AUTO_INCREMENT,
    numero_factura VARCHAR(30) NOT NULL,
    fecha TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    subtotal DECIMAL(10,2) NOT NULL,
    total DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_facturas_numero_factura (numero_factura),
    CONSTRAINT chk_facturas_subtotal CHECK (subtotal >= 0),
    CONSTRAINT chk_facturas_total CHECK (total >= 0)
) ENGINE=InnoDB;

-- Índice para consultas por fecha
CREATE INDEX idx_facturas_fecha ON facturas(fecha);

CREATE TABLE factura_detalles (
    id INT NOT NULL AUTO_INCREMENT,
    factura_id INT NOT NULL,
    producto_id INT NOT NULL,
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    subtotal DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT chk_factura_detalles_cantidad CHECK (cantidad > 0),
    CONSTRAINT chk_factura_detalles_precio_unitario CHECK (precio_unitario >= 0),
    CONSTRAINT chk_factura_detalles_subtotal CHECK (subtotal >= 0),
    CONSTRAINT fk_factura_detalles_factura
        FOREIGN KEY (factura_id)
        REFERENCES facturas(id)
        ON DELETE CASCADE
        ON UPDATE CASCADE,
    CONSTRAINT fk_factura_detalles_producto
        FOREIGN KEY (producto_id)
        REFERENCES productos(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
) ENGINE=InnoDB;

-- Índices
CREATE INDEX idx_factura_detalles_factura_id ON factura_detalles(factura_id);
CREATE INDEX idx_factura_detalles_producto_id ON factura_detalles(producto_id);

-- ============================================
-- DATOS DE PRUEBA
-- ============================================
INSERT INTO productos (codigo, nombre, descripcion, precio, stock)
VALUES
('P001', 'Arroz Selecto 5lb', 'Arroz premium de 5 libras', 350.00, 20),
('P002', 'Aceite Vegetal 1L', 'Botella de aceite vegetal', 125.00, 15),
('P003', 'Habichuelas Rojas 2lb', 'Paquete de habichuelas rojas', 95.00, 30);

INSERT INTO facturas (numero_factura, fecha, subtotal, total)
VALUES
('FAC-0001', CURRENT_TIMESTAMP, 700.00, 700.00),
('FAC-0002', CURRENT_TIMESTAMP, 375.00, 375.00);

INSERT INTO factura_detalles (factura_id, producto_id, cantidad, precio_unitario, subtotal)
VALUES
(1, 1, 2, 350.00, 700.00),
(2, 2, 3, 125.00, 375.00);

SHOW TABLES;

SELECT * FROM productos;

-- Ver facturas con su detalle
SELECT
     f.numero_factura,
     f.fecha,
     p.nombre AS producto,
     fd.cantidad,
     fd.precio_unitario,
     fd.subtotal
 FROM facturas f
 INNER JOIN factura_detalles fd ON f.id = fd.factura_id
 INNER JOIN productos p ON p.id = fd.producto_id
 ORDER BY f.fecha DESC;

-- Reporte básico de ventas por rango de fechas
SELECT
     COUNT(*) AS cantidad_facturas,
     COALESCE(SUM(total), 0) AS total_vendido
 FROM facturas
 WHERE fecha BETWEEN '2026-03-01 00:00:00' AND '2026-03-31 23:59:59';

-- Reporte detallado por rango de fechas
 SELECT
     f.numero_factura,
     f.fecha,
     f.total
 FROM facturas f
 WHERE f.fecha BETWEEN '2026-03-01 00:00:00' AND '2026-03-31 23:59:59'
 ORDER BY f.fecha DESC;