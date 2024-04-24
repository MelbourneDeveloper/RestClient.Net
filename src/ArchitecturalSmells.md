# Análisis de Deuda de Arquitectura


## Analisis general usando Designite para C#
- **Lineas de codigo analizadas**: 8935
- **Numero de Tipos**: 133
- **Cantidad de Namespaces**: 17
- **Cantidad de Metodos**: 451
- **Densidad de CodeSmells por KLOC**: 13,21
- **Porcentaje de duplicacion de codigo**: 2,5
- **Numero de violaciones de metricas**: 36

## Design Smells
- **Abstraction Smells**: 80
	- El mas comun son las abstracciones no utilizadas.
	- El segundo mas comun es la abstraccion innesesaria.
	- 13 casos de Modularizacion Ciclicamente Dependiente.
	- 14 casos de abstracciones duplicadas.
	- 8 casos de herarquia rebelde.
	- 11 casos de Modularizacion rota.
- **Encapsulation Smells**: 0
- **Modularization Smells**: 29
- **Hierarchy Smells**: 9

## Architecture Smells
- **Feature Concentration**: 4
	- La herramienta detectó el problema en este componente porque el componente realiza más de una preocupación/característica arquitectónica.

## Otras metricas detectadas por Designite
- Hay un namespace duplicado llamado "RestClient.net" sin una distinción aparente de su proposito. Otra herramienta de Estructura de matrices de dependencias detectó 5 repeticiones del mismo namespace, lo cual hace dificil de leer y apartar los resultados.
- Los dos RestClients aportan una gran cantidad de architecture smells con 1,34 y 1,44 unidades de smell por KLOC. Y entran en la categoria de Muy Grave.
- La clase "ApiExamples.Controllers" es la que mas aporta architecture smells con 3,92 unidades de smell por KLOC. Y entra en la categoria de Muy Grave.
- Por ultimo la clase "UnitTest" aporta una cantidad normal de architecture smells con 0,34 unidades de smell por KLOC. Y entra en la categoria de Normal.
- Al organizarse por proyecto, Abstractions tiene 39 dependencias con el proyecto RestClient.Net y 65 con el proyecto de UnitTests. Esto no es necesariamente malo, solo que puede ser sintoma de gran acoplacion entre los proyectos.

## Analisis Manual
- **Arquitectura**: El proyecto principal cuenta con arquitectura de MVC pero las otras bibliotecas no cuentan con una arquitectura clara o al menos bien organizada y lo cual puede involucionar en una Gran Bola de Lodo.
	- Nota extra: En el proyecto base ya hay Issues que hablan de la necesidad de mejor rendimiento y velocidad, lo cual indica una arquitectura no optima ni escalable.
- **Documentación**: No se cuenta con una documentación técnica que explique la arquitectura del proyecto y como se puede extender, modificar o desplegarlo. Ademas hay clases que no cuentan con documentación de su proposito y uso.
	- Ejemplo: Stuff.cs, esta clase pareciera como si fuera basura pero es dificil determinar su proposito sin documentación.
- **Recomendaciones**: Se propone reorganizar las bibliotecas y clases para que sigan una arquitectura clara y escalable. 
	- **Mejoras Propuestas**: Se propone usar una arquitectura Basada en Capas, la cual divide el proyecto en capas por funcionalidad y responsabilidad. Se recomienda no seguir usando la arquitectura MVC y cambiar por otra que permita una mejor escalabilidad y rendimiento.

## Conclusiones
- **Designite** es una herramienta muy util para detectar code smells y architecture smells en un proyecto de C#. Limitaciones de la herramienta son que no detecta todos los problemas y no da una solución a los problemas detectados. Por ejemplo no analiza la organización de los proyectos, especialmente los nombres de archivos, namepaces la orgnización de carpeta.
	- La limitación mas grande es que sus lecturas las hace a partir de una arquitectura ideal que puede o no ser usada en el proyecto. Por ejemplo, esta de ser una aplicacion nativa de nube tendria problemas analizando y haciendo recomendaciones utiles sobre esta arquitectura poco utilizada.
	- Es una herramienta util pero todavia bastante dependiente de la interpretación humana y factores mas subjetivos como la organización de los proyectos y las malas practicas.
