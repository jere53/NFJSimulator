# NFJSimulator

## Como descargar el Proyecto
El proyecto utiliza la version 2021.2.0b13 de Unity. Asegurarse de tener Unity Hub y esta version de Unity instalada.

Clonar el repositorio, luego abrir Unity Hub, seleccionar "ADD", y en el explorador elegir la carpeta en la cual se clono el repositorio.

Descargar [este](https://drive.google.com/file/d/10g3pHxck5nuki5l9KdwhPd8k1atlobLS/view?usp=sharing) Unity Package para conseguir el resto de los Assets.

Abrir el proyecto desde Unity Hub, si dice que hay errores en el codigo elegir la opcion "ignorar", luego importar el Unity Package descargado anteriormente.

En la carpeta /Assets/Unvertioned/Scenes se podra encontrar la escena "GTA-PeatonesNoCominada" que contiene la funcional desarrollada hasta el momento.

### Importante
En las opciones del proyecto, asegurarse que esten seleccionados "Visible Meta Files" y "Asset Serialization: Force Text"

## Como controlar las versiones de codigo y otros archivos de texto
Por cuestiones de espacio, no todos los archivos pueden ser versionados. Los modelos 3D, imagenes, y archivos de audio, por ejemplo, son demasiado grandes para versionar utiliando GitHub.

Por esta razon, el proyecto Unity estara divido en 2 partes. Una parte "Versioned" y una parte "Unversioned"

Dentro de la carpeta "Assets", se deberan producir 2 carpetas:
> La carpeta "Versioned" contendra todos los archivos a ser versionados con GitHub. Es decir, los Scripts y otros archivos de texto, como READMEs y documentacion.

> La carpeta "Unversioned" contendra todo lo demas. Modelos, imagenes, audio, escenas, paquetes, etc.
Dentro de estas carpetas se pueden usar subcarpetas como se desee, pero es obligatorio que la carpeta Assets este subdividida en estas 2 y solamente estas 2 carpetas.

## Como controlar las versiones de otros tipos de archivos
Naturalmente, cada uno debera agregar modelos, configurar prefabs, descargar otros paquetes, y armar sus propias escenas. Para versionar estos tipos de archivos,
cada vez que alguien comienze a trabajar en su "branch", creara una nueva carpeta para los archivos de este tipo que desee agregar dentro de la carpeta "Unversioned".

Esta carpeta debe llevar el nombre de la "branch" en cuestion. En esta carpeta se deberan guardar TODOS los archivos (organizados en subcarpetas si asi se desea) que se agregen durante el desarrollo en esa "branch" y no sean del primer tipo descrito (Scripts y Documentacion).

Si se deasease modificar uno de los archivos "Unversioned", entonces ese archivo primero sera COPIADO desde su ubicacion original a la nueva carpeta creada para la branch. Luego se realizaran las modificaciones necesarias, y durante el merge el archivo original sera REEMPLAZADO por el archivo presente en la carpeta del branch. Debido a esto es importante notificar si se realizaran modificaciones a un archivo "Unversioned", para que nadie mas trabaje sobre ese archivo.

Ejemplo: Si se esta trabajando en la branch "b_de_j_12" y se desea agregar un archivo "Imagen1", Imagen1 debera estar en /Assets/Unversioned/b_de_j_12/Imagen1. Si se desease modificar el archivo "Video1", encontrado en /Assets/Unversioned/Video1, entonces se debera copiar Video1 a /Assets/Unversioned/b_de_j_12/Video1COPIA y modificar esa copia (previamente habiendo avisado que se modificara Video1, para evitar conflictos).
Luego durante el merge /Assets/Unversioned/Video1 sera REEMPLAZADO por /Assets/Unversioned/b_de_j_12/Video1COPIA.

Si por alguna razon debieran mantenerse ambas versiones de Video1, entonces esto se resolvera durante el merge, cambiando el nombre de una y modificando el codigo si es necesario.

## El Merge
El codigo y la documentacion pueden ser mergeados usando Git, pero para lo "Unversioned" el proceso sera el siguiente: 

> Inicialmente, todos conseguiran la misma carpeta "Unversioned" descargando un Unity Package disponible [aqui](https://drive.google.com/file/d/10g3pHxck5nuki5l9KdwhPd8k1atlobLS/view?usp=sharing)
  
> Los miembros crean las branches necesarias para su trabajo
  
> Cuando se desean mergear los cambios de una rama, se realiza el Pull Request en GitHub para mergear el codigo a la rama base.
  
> Luego, cuando uno quiera hacer el "pull request" de su Unversioned, se debera construir un Paquete Unity con la carpeta correspondiente a ese branch. Por ejemplo, si quiero mergear lo "Unversioned" de la branch "b_de_j_12", exportar un Unity Package con la carpeta "/Assets/Unversioned/b_de_j_12". Ese Unity Package se subira a una carpeta con en el nombre de la branch dentro de [este Drive](https://drive.google.com/drive/folders/1ReQVB0rvMVVJBeyd1RiOm2vcusxNZCiy?usp=sharing), para que los otros miembros del equipo puedan descargarlo. Si es necesario reemplazar uno de los archivos en "Unversioned" cono uno de los de ese Unity Package, esto se le notificara al resto del equipo para que puedan realizar ese reemplazo.
  
> De esta manera, cada rama tendra su Codigo y Documentacion en GitHub, y su Unity Package correspondiente en [Drive](https://drive.google.com/drive/folders/1ReQVB0rvMVVJBeyd1RiOm2vcusxNZCiy?usp=sharing)

## Las ramas
La rama "main" contiene la baseline del proyecto
La rama "nfjsimulator_v1.1" es la rama base para el trabajo realizado durante el Sprint 1. Sera el destino de los Pull Request duante el Sprint 1

Las ramas que se creen para el trabajo durante el Sprint 1 deben ser ramas de "nfjsimulator_v1.1". Para esto, al crear la rama en la terminal escribir "git checkout -b <nombre_Rama> nfjsimulator_v1.1". Esto hara que se branchee desde "nfjsimulator_v1.1" y no desde "main".
