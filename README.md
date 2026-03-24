# ChArDi POS System

## Información académica
- **Participantes:**
  - Christian Felix Gil Castillo — 2012-1036
  - Cyd Marie Jorge Chapman — 2024-0006
- **Carrera:** Desarrollo de Software
- **Materia:** Introducción a la Ingeniería de Software
- **Facilitador:** Eduandy Isabel Cruz Abreu

## Descripción
ChArDi POS System es un sistema web de punto de venta orientado a pequeños negocios de la República Dominicana, como colmados, minimarkets, granjas y tiendas locales. Su propósito es mejorar el control de ventas, inventario y facturación mediante una solución tecnológica integrada que reduzca inconsistencias operativas, fortalezca la trazabilidad de las transacciones y sirva como base para futuros módulos del sistema.

El MVP del proyecto se enfoca en:
- Registro y gestión de productos
- Generación de facturas
- Actualización automática del inventario tras cada venta
- Consulta de reportes básicos de ventas por rango de fechas

## Objetivo académico y técnico
Este repositorio forma parte del desarrollo del proyecto basado en TDD para la asignatura Introducción a la Ingeniería de Software. El enfoque de trabajo parte de historias de usuario, criterios de aceptación y casos de prueba definidos previamente para construir una base funcional del sistema con mayor calidad, trazabilidad y mantenibilidad.

## Funcionalidades del PMV
El Producto Mínimo Viable contempla tres bloques funcionales principales:

1. **Gestión de productos**  
   Permite registrar y modificar productos con validaciones obligatorias, controlando nombre, código, precio y stock.

2. **Generación de facturas**  
   Permite seleccionar productos disponibles, calcular el total automáticamente, registrar la venta y reducir el inventario de forma transaccional.

3. **Reporte de ventas por rango de fechas**  
   Devuelve el total vendido y la cantidad de facturas en el período consultado.

## Enfoque TDD
El desarrollo del proyecto sigue la metodología Test Driven Development:
1. Se redacta una prueba que falla.
2. Se implementa el mínimo código necesario para hacerla pasar.
3. Se refactoriza sin alterar el comportamiento observable.

Este ciclo se aplica especialmente en reglas como:
- Registrar productos válidos
- Rechazar precios o stocks negativos
- Crear facturas válidas
- Evitar ventas con stock insuficiente
- Validar reportes por fecha

## Tecnologías utilizadas
El proyecto está planteado con:
- **Backend:** ASP.NET Core
- **Frontend:** React (planificado dentro del alcance del proyecto)
- **Base de datos:** PostgreSQL (plan inicial documental)
- **Persistencia:** Entity Framework Core
- **Calidad:** pruebas unitarias e integración (enfoque TDD)

## Organización del equipo
El equipo de trabajo está compuesto por Christian Gil y Cyd Jorge.

- **Christian Felix Gil Castillo**: Product Owner y Backend Developer.
- **Cyd Marie Jorge Chapman**: Scrum Master y Frontend Developer.

## Historias de usuario principales
### Gestión de productos
Como administrador, quiero registrar y modificar productos para mantener un inventario actualizado.

### Generación de facturas
Como cajero, quiero generar facturas para registrar ventas y actualizar el inventario automáticamente.

### Reporte de ventas
Como gerente, quiero generar un reporte de ventas por rango de fechas para analizar el desempeño del negocio.

## Casos de prueba considerados
El diseño del proyecto contempla pruebas para:
- Registrar productos válidos
- Rechazar nombre vacío
- Impedir precio o stock negativo
- Modificar productos existentes
- Crear facturas válidas
- Calcular el total de forma automática
- Reducir stock al facturar
- Evitar ventas sin inventario suficiente
- Impedir facturas sin detalles
- Generar reportes válidos
- Contar facturas y total vendido correctamente
- Validar rangos de fechas incorrectos

## Flujo de CI/CD solicitado en la tarea
Se incorpora un flujo sencillo de integración y despliegue continuo en GitHub Actions para automatizar un pipeline básico sobre eventos `push` y `pull_request`.

## Workflow esperado
Ubicación:

```text
.github/workflows/ci-pipeline.yml
```

Contenido base sugerido:

```yaml
name: CI Pipeline
on: [push, pull_request]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Clonar repositorio
        uses: actions/checkout@v3
      - name: Configurar entorno
        run: echo "Configurando entorno de compilación"
      - name: Compilar proyecto
        run: echo "Ejecutando compilación"
```

## Control de versión esperado
Para cumplir la práctica:
- `main` como rama principal
- `develop` como rama de integración
- `feature/*` para nuevas funcionalidades

Además, la entrega requiere al menos un commit y un pull request como evidencia del flujo colaborativo.

## Cómo ejecutar el workflow en GitHub
1. Subir el código al repositorio.
2. Agregar el archivo del workflow en la ruta correspondiente.
3. Hacer `push` o abrir un `pull_request`.
4. Verificar la ejecución en la pestaña **Actions**.

## Estado actual del proyecto
Según la documentación del sprint, el proyecto ya logró avances en conexión a base de datos, persistencia con EF Core, lógica transaccional para reducción de stock, validación de errores e integración entre capas. Como prioridad futura se plantea fortalecer JWT, control de roles, documentación técnica formal y reportes más avanzados.

## 📄 Licencia
Este proyecto se distribuye bajo la licencia MIT, lo que significa que puedes utilizarlo, modificarlo y distribuirlo libremente.
