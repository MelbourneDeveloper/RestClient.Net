# Analisis de experiencia de Desarrollador

## DevExperience
- **Estado del Flujo**:
	- Por un lado ya que es un proyecto que lo mantiene una sola persona se puede ver que el flujo de trabajo es constante y no hay interrupciones. Sin embargo, las organizaciones generalmente ponen friccion en el flujo para asegurar ciertos estándares de calidad y seguridad. En este caso, el desarrollador es el unico que revisa y autoriza sus propios PR y esto resulta en una falta de revision por pares.
	- Por otro lado al priorizar un flujo rápido se termina obteniendo uno mas lento por las deudas tecnicas generadas por la falta de revision por pares y la falta de documentacion.
- **Ciclos de Retroalimentacion**:
	- El código es de un proyecto pequeño y se corre y de despliega rápido y da retroalimentacion rapida.
	- Sin embargo, otras fuentes de retroalimentacion rápidas como son el IDE, los flujos dentro de Github son ignorados a proposito lo cual compromete las respuestas de los sistemas automatizados.
- **Carga Cognitiva**:
	- Se puede ver que esta parte tambien necesita mejora pues directamente hay lineas de código que el mismo desarrollador no sabe que hacen. 
	- Los analizadores de codigo descubrieron una alta complejidad en la lógica principalmente porque se le está intentando dar soporte a versiones antiguas de .net lo cual compromente altamente la legibilidad.
	- La falta de documentación tambien aumenta la carga cognitiva pues el desarrollador tiene que recordar todo lo que ha hecho y no puede consultar la documentacion.


## SPACE Framework
El Framework de space fue creado dado que la productividada es multidimensional y no se puede medir con una sola metrica. Asi que se identificaron 5 dimensiones con las que vamos a analizar este proyecto.
- **Safisfaccion y Bienestar**: Un desarrollador con baja satisfaccion indica posible Burnout y baja productividad.
	- En este proyecto se pueden ver sintomas pues el desarrollador no actualiza el proyecto hace mas de un año ni responde a los issues cuando hay problemas y mejoras muy aparentes.
- **Performance**: 
	- La caliad de codigo es baja y hace falta mucha documentacion. El impacto por el otro lado es alto pues es una libreria muy usada con hasta 10mil descargas totales.
- **Actividad**:
	- Durante la mayor parte del 2023 el proyecto estuvo muy activo gracias a la gran cantidad de fixes y 48 Pull Requests sobre el main. Cuenta con 10 workflows en Github Actions pero algunos fallaron y no se han actualizado.
- **Comunicacion y Colaboracion**:
	- Como es un proyecto open source mantenido principalmente por una persona es dificil determinar la comunicacion y colaboracion. Sin embargo, se puede ver que hay 8 colaboradores que aportaron al proyecto asi sea poco. Hay un problema muy grade con los PR pues la mayoria son automaticos del Dependabot y el mismo desarrollador expreso que se le perdio un PR por falta de comunicacion.
	- Tambien se puede ver en el historial de los issues que las descripciones y respuestas tienen a ser pobres y se podria mejorar la claridad de los problemas y soluciones. Ademas la falta de documentacion hace que sea dificil para los nuevos colaboradores entender el proyecto.
	- Tambien se ve una falta de revision por pares pues el desarrollador es el unico que revisa y autoriza sus propios PR.
	- Da la sensasion que este fue un proyecto entre conodidos y no fue pensado para ser un proyecto verdaderamente open source.
- **Eficiencia y Flujo**:
	-  No hay suficiente informacion para determinar la eficiencia y flujo del proyecto a partir de la informacion disponible.

## Conclusiones: 
	- El proyecto tiene un gran impacto pero la calidad del codigo y la documentacion es baja.
	- La comunicacion y colaboracion es pobre y esta parece ser la dimension que mas afecta el proyecto.
	- Se puede concluir que la satisfaccion del desarrollador es baja y se puede ver que el proyecto esta abandonado.
	- Se recomienda que el desarrollador busque ayuda para mantener el proyecto o que lo archive si no puede mantenerlo.
	- Tambien se recomienda volver el proyecto verdaderamente open source para poder mejorar la comunicacion, colaboracion y posiblemente conseguir algun sponsor.
	- Para poder lograr un mejor analisis segun el framework de SPACE se recomienda medir las exeriencias subjetivas de los colaboradores y usuarios del proyecto pues puede proveer mejor informacion sobre la satisfaccion y bienestar del desarrollador y la calidad del trabajo.



