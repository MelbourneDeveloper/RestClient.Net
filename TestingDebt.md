# Análisis de Deuda Tecnica y Recomendaciones

## Tipos de Deuda Tecnica en el Proyecto
- **Requerimientos**: Parece que el proyecto cumple con su proposito y los usuarios no han reportado problemas con el mismo.
- **Arquitectura**: El proyecto principal cuenta con arquitectura de MVC pero las otras bibliotecas no cuentan con una arquitectura clara o al menos bien organizada y lo cual puede involucionar en una Gran Bola de Lodo.
	- Nota extra: En el proyecto base ya hay Issues que hablan de la necesidad de mejor rendimiento y velocidad, lo cual indica una arquitectura no optima ni escalable.
- **Diseño**: El diseño del proyecto depende mucho de una clase Dios llamada Client.cs la cual tiene 300 lineas de código la cual viola el principio de responsabilidad única ya que una entidad no debería tener aparte de sus propiedades tanta lógica de negocio.
	- Ejemplo: Client.cs
- **Código**: Gran parte del código fuente está dedicado a desactivar las reglas de análisis de código del IDE y de SonarQube. Esto es un problema grave pues no se puede determinar la calidad del código fuente y se presta para que se introduzcan malas prácticas en el código.
	- Ejemplo Común:  #pragma warning disable CA1002
- **Pruebas**: Ver sección de Pruebas Unitarias.
- **Despliegue**: El código se pudo desplegar como paquete Nuget sin problema desde el 2016. Pero existen problemas con las dependencias que se necesitan actualizar pues cuentan con vulnerabilidades. Aparte de eso el proyecto hace buen uso del administrador de paquetes Nuget de .Net el cual es un estandar en la industria.
- **Documentación**: Existe documentación que explica como usar el proyecto y como contribuir al mismo. Pero no se cuenta con una documentación técnica que explique la arquitectura del proyecto y como se puede extender, modificar o desplegarlo. Ademas hay clases que no cuentan con documentación de su proposito y uso.
	- Ejemplo: Stuff.cs, esta clase pareciera como si fuera basura pero es dificil determinar su proposito sin documentación.
- **Versionamiento**: El proyecto cuenta con Github para el versionamiento del código pero este no se esta utilizando correctamente pues aunque mediante los Tags expresen los releases, no se esta utilizando correctamente el sistema de ramas para el desarrollo del proyecto. Solo cuenta con la rama Main activa y las otras estan altamente desactualizadas y no corresponden a los releases.

## Pruebas Unitarias
- **Cobertura de Pruebas**: Segun la página principal el proyecto cuenta con una cobertura del 100% del código. Este número es el declarado, pero no se sabe con exactitud pero si se sabe que hay codigo que no está siendo referenciado y menos probado.
- **Cantidad de Pruebas**: Se cuenta con 669 pruebas unitarias totales.
- **Problema**: El código fuente cuenta con problemas en su libreria y en estandares de código que no deja correr ninguna prueba unitaria.
- **Conclusión**: No se puede determinar si las pruebas unitarias son correctas o no ni la calidad de estas.

## Mejoras propuestas para reducir la Deuda Tecnica
- **Requerimientos**: No se encontraron problemas con los requerimientos del proyecto.
- **Arquitectura**: Se propone reorganizar las bibliotecas y clases para que sigan una arquitectura clara y escalable. Se recomienda no seguir usando la arquitectura MVC y cambiar por otra que permita una mejor escalabilidad y rendimiento.
	- **Recomendación**: Se propone usar una arquitectura Basada en Capas, la cual divide el proyecto en capas por funcionalidad y responsabilidad.
- **Diseño**: Se propone reorganizar la clase Client.cs para que siga el principio de responsabilidad única. No deberia haber razón para apagar las advertencias del IDE y de SonarQube.
	- **Recomendación**: Dividir la clase Client.cs en clases más pequeñas que sigan el principio de responsabilidad única.
- **Código**: Se propone quitar todo el código relacionado con apagar las advertencias del IDE y de SonarQube ya que esto no permite determinar la calidad del código fuente.
	- **Recomendación**: Se recomienda seguir las reglas de análisis de código del IDE y de SonarQube para mejorar la calidad del código fuente.
	- **Recomendación**: Se deben seguir las advertencias del IDE y de SonarQube que si se pueden detectar pues son faciles de corregir y mejoran la calidad del código fuente.
- **Pruebas**: Se propone arreglar los problemas en el código fuente para que se puedan correr las pruebas unitarias.
	- **Recomendación**: Se recomienda arreglar los problemas en el código fuente para que se puedan correr las pruebas unitarias y determinar si estas son correctas o no.
- **Despliegue**: Se propone actualizar las dependencias del proyecto para que no cuenten con vulnerabilidades.
	- **Recomendación**: Se recomienda actualizar las dependencias del proyecto para que no cuenten con vulnerabilidades y se pueda desplegar sin problemas.
- **Documentación**: Se propone documentar las clases que no cuentan con documentación de su proposito y uso.
	- **Recomendación**: Se recomienda documentar las clases que no cuentan con documentación de su proposito y uso para que se pueda entender mejor el proyecto.
- **Versionamiento**: Se propone utilizar correctamente el sistema de ramas y versionamiento para el desarrollo del proyecto.
	- **Recomendación**: Se recomienda utilizar correctamente el sistema de ramas para el desarrollo del proyecto para que se pueda seguir el progreso del proyecto y se pueda trabajar en paralelo en diferentes funcionalidades y sea mas claro el código fuente.