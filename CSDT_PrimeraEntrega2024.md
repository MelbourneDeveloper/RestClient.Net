# Análisis de Calidad del Código y Recomendaciones

## Resultados de SonarQube

- **Code Smells**: 49
- **Complejidad Ciclomática**: 307
- **Complejidad Cognitiva**: 119
- **Deuda Técnica diagnosticada**: 3h 34min
- **Duplicaciones**: 0%
- **Cobertura de pruebas**: 0%
- **Ratio de deuda**: 0.5% A
- Se sugiere dividir 2 clases para mejorar la legibilidad del código.
- En promedio, el 11% del código son comentarios.
- Hay una cantidad significativa de TODOs sin resolver.
- Problemas detectados en el código:
  - **Bloqueantes**: 1
  - **Críticos**: 2
  - **Mayores**: 39
  - **Menores**: 4
  - **Informativos**: 3
	- 
## Resultados de las pruebas unitarias presentes en el proyecto
- **Pruebas unitarias**: 669
- **Pruebas fallidas**: 0
- **Pruebas que no pudieron iniciar**: 669
- **Cobertura de pruebas**: 0%
- Ya que las pruebas no pudieron iniciar, no se puede determinar la cobertura de pruebas ni su efectividad.


## Resultados de Github Copilot Chat
	- Se detectó uso Excesivo de desactivación de advertencias relacionadas con diferentes code smells
	- Las categorias mas desactivadas son:
		- **Nombramiento de variables**: Se relacionan con la nomenclatura de las variables.
		- **Inicialización de variables**: Se relacionan con la no inicialización de las variables no nulas.
		- **Correcto uso de los tipos de datos**: Se relacionan con el uso correcto de los tipos.
	- Se detectó caso omiso deliveradamente a las advertencias del IDE, incluso cuando estas estan marcadas como errores.
	- Se detectó una clase Dios, la cual tiene una cantidad excesiva de métodos y atributos, alrededor de 350 lineas de código para la clase principal.


## Resultados de Visual Studio

- **Advertencias**: 67
- **Errores**: 37
- **Mensajes**: 3
- El limpiador y formateador automático de código modificó 3 archivos.
- En vez de lanzar excepciones mas especificas, se lanzan excepciones generales.
- Se detectó uso excesivo de desactivación de advertencias relacionadas con diferentes code smells.
- Se desactivan advertencias en lugares donde no se podrian generar esos errores indicados por las advertencias.
- Se detectó caso omiso deliveradamente a las advertencias del IDE, incluso cuando estas estan marcadas como errores.
- Falta de organización en los controladores donde las acciones no estan agrupadas por funcionalidad [Get, Post, Get?]
- Se detectó mucho código que no se esta utilizando en el proyecto. 

## Resultados de Snyk.io (Vulnerabilidades)
- **RestClient.Net.Samples.Uno.Droid.csproj .NetFramework Version 8**: Alto 6, Medio 1
- **RestClient.Net.Samples.Uno.Wasm.csproj netstandard2.0**: Alto 6, Medio 1
- **RestClient.Net.Samples.UnitTests.csproj net45**: Alto 5, Medio 1
- **RestClient.Net.UnitTests.csproj net6.0**: Alto 5, Medio 1
- **RestClient.Net.UnitTests.csproj netcoreapp3.1**: Alto 5, Medio 1
- **RestClient.Net.UnitTests.csproj net5.0**: Alto 5, Medio 1
- **RestClient.Net.UnitTests.csproj net7.0**: Alto 5, Medio 1
- **RestClient.Net.PerformanceTests.csproj netcoreapp3.1**: Alto 2, Medio 1
- **Code Analysis**: Alto 1, Low 16
- **RestClient.Net.Samples.csproj netstandard2.0**: Alto 1
- **RestClient.Net.CoreSample.csproj net6.0**: Alto 1
- **ApiExamples.csproj netcoreapp3.1**: Alto 1

## Resultados de Pruebas Manuales

- La instalación del proyecto presenta gran complejidad.
  - Vulnerabilidad de alto riesgo en Newtonsoft.Json.
  - Discrepancias en las versiones de librerías instaladas.
  - Compatibilidad solo con versiones descontinuadas de .NET Core (2.0 y 4.5).
  - Falta de compatibilidad con versiones de .NET Core superiores a la 7.
- Falta de buenas prácticas.
  - Insuficiente documentación y comentarios en el código.
  - Pruebas unitarias existentes no son detectadas.
  - Desorganización en el proyecto de pruebas.
  - Se detectó TODOs sin resolver.
  - Hay poco uso de carpetas para organizar y agrupar el código.
  - Hay mucho codigo dedicado a suprimir advertencias del IDE.
  - Los namespaces no corresponden a la estructura de carpetas y la advertencia se suprime.
  - La arquitectura Modelo Vista Controlador solo se aplica en ciertas partes del proyecto.
  - Esta joya: 
	- //Note: not sure why this is necessary...
	- RestClient.Net/DefaultGetHttpRequestMessage.cs Linea 69.
	- Clase llamada "Stuff.cs" con ningún comentario que la explique.
	- //Is this good?
	- RestClient.Net.Abstractions/CallExtensions.cs Linea 66
	

## Recomendaciones

- **Refactorización**: Priorizar la resolución de los problemas bloqueantes y críticos. Considerar dividir las clases sugeridas para mejorar la legibilidad.
- **Negligencia de advertencias**: Revisar y corregir las advertencias desactivadas en el código. Asegurarse de que no se estén ignorando problemas importantes especialmente si el mismo IDE las marca.
- **Documentación**: Aumentar la cantidad de comentarios en el código, especialmente en partes complejas y para TODOs importantes.
- **Pruebas**: Mejorar la cobertura de pruebas automatizadas. Reorganizar y optimizar el proyecto de pruebas para asegurar su efectividad.
- **Dependencias**: Actualizar o reemplazar la librería Newtonsoft.Json por una versión sin vulnerabilidades conocidas. Unificar versiones de librerías para evitar conflictos.
- **Compatibilidad**: Asegurar la compatibilidad del proyecto con versiones recientes de .NET Core para garantizar seguridad y soporte a largo plazo.
- **Organizacion**: Organizar los controladores de acuerdo a su funcionalidad y agrupar las acciones por tipo de solicitud HTTP.
- **Eliminación de código no utilizado**: Eliminar el código que no se esta utilizando en el proyecto o que no esté referenciado por otros componentes.
- **Descontinuar soporte de versiones antiguas**: Eliminar soporte para versiones de .NET Core descontinuadas y migrar a versiones más recientes para aprovechar mejoras y correcciones de errores ademas de limpiar el proyecto.
- **Agrupacion de codigo**: Organizar el código en carpetas y agrupar por funcionalidad para facilitar la lectura y mantenimiento del código.
- **Pruebas unitarias**: Asegurarse de que las pruebas unitarias se puedan ejecutar correctamente pues no importa si hay cobertura del 100% si no se pueden ejecutar.

