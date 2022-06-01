using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Window_Graph : MonoBehaviour
{
    private RectTransform GraphContainer { get; set; }
    
    //sprite que tendran los puntos del grafico de lineas
    //[SerializeField] private Sprite dotSprite;

    //referencias a los GameObjects que tienen las plantillas de los separadores y etiquetas
    private RectTransform _labelTemplateX;
    private RectTransform _labelTemplateY;
    private RectTransform _dashTemplateX;
    private RectTransform _dashTemplateY;
    private RectTransform _markValueTemplateY;

    //listas con los objetos que instancia el ShowGraph para borrarlos si se vuelve a invocar el metodo.
    private List<GameObject> _gameObjects;
    private List<IGraphVisualObject> _graphVisualObjectList;

    //referencia al GO con la plantilla de la tooltip
    private GameObject _tooltipGameObject;

    //el eje Y del grafico debe comenzar en 0?
    private bool _startYScaleAtZero;

    //lista con las etiquetas instanciadas por el ShowGraph, para actualizarlas cambia la escala.
    private List<RectTransform> _yLabelList;
    
    //Valores Cacheados (en una invocacion al ShowGraph)
    private List<float> _cachedValueList;
    private IGraphVisual _cachedGraphVisual;
    private int _cachedMaxVisibleAmount;
    private Func<float, string> _cachedGetAxisLabelX;
    private Func<float, string> _cachedGetAxisLabelY;
    private float _cachedXSize;
    private List<float> _cachedXAxisLabelList;

    public RectTransform GetGraphContainer()
    {
        return GraphContainer;
    }
    
    private void Awake()
    {
        //inicializar estructuras
        _graphVisualObjectList = new List<IGraphVisualObject>();
        _gameObjects = new List<GameObject>();
        _yLabelList = new List<RectTransform>();
        _cachedXAxisLabelList = new List<float>();
        
        //conseguir referencias a las plantillas
        GraphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        _labelTemplateX = GraphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        _labelTemplateY = GraphContainer.Find("labelTemplateY").GetComponent<RectTransform>();

        _dashTemplateX = GraphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        _dashTemplateY = GraphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        _markValueTemplateY = GraphContainer.Find("markValueTemplateY").GetComponent<RectTransform>();
        
        _tooltipGameObject = GraphContainer.Find("Tooltip").gameObject;

        /*
        //inicializar visualizaciones del grafico (para testeo)
        LineGraphVisual lineGraphVisual =
            new LineGraphVisual(this, _graphContainer, _dotSprite, Color.green, 
                new Color(1, 1, 1, .5f));
        
        BarChartVisual barChartVisual = new BarChartVisual(this, _graphContainer, Color.green, .8f);
        
        //configurar botones para cambiar aspecto visual del grafico (para testeo)
        transform.Find("Boton BarChart").GetComponent<Button>().onClick.AddListener(() =>
        {
            SetGraphVisual(barChartVisual);
        });
        transform.Find("Boton LineGraph").GetComponent<Button>().onClick.AddListener(() =>
        {
            SetGraphVisual(lineGraphVisual);
        });
        
        //configurar botones para cambiar la cantidad de objetos desplegados por el grafico, y para cambiar las etiquetas
        //(para testeo)
        transform.Find("Boton ReducirCantidadValoresVisibles").GetComponent<Button>().onClick.AddListener(DecreaseVisibleAmount);

        transform.Find("Boton AumentarCantidadValoresVisibles").GetComponent<Button>().onClick.AddListener(IncreaseVisibleAmount);

        transform.Find("Boton USD").GetComponent<Button>().onClick.AddListener(() =>
        {
            SetGetAxisLabelY(f => "$" + Mathf.RoundToInt(f));
        });
        transform.Find("Boton EUR").GetComponent<Button>().onClick.AddListener(() =>
        {
            SetGetAxisLabelY(f => "€" + Mathf.RoundToInt((f/1.18f)));
        });
        
        */
    }

    private void OnDisable()
    {
        //clean uo
        foreach (var g in _gameObjects)
        {
            Destroy(g);
        }
        _gameObjects.Clear();
        
        foreach (var graphVisualObject in _graphVisualObjectList)
        {
            graphVisualObject.CleanUp();
        }
        _graphVisualObjectList.Clear();
        
        _yLabelList.Clear();
    }

    #region Show/Hide Tooltips
    
    /*
     * Funcion que muestra una tooltip, basada en la plantilla "_tooltipGameObject".
     * Con texto "tooltipText" en la posicion "anchoredPosition"
     */
    public void ShowTooltip(string tooltipText, Vector2 anchoredPosition)
    {
        _tooltipGameObject.SetActive(true);

        _tooltipGameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
        
        float textPaddingSize = 4f;
        Text tooltipTextComponent = _tooltipGameObject.transform.Find("text").GetComponent<Text>();
        tooltipTextComponent.text = tooltipText;
        Vector2 backgroundSize = new Vector2(tooltipTextComponent.preferredWidth + textPaddingSize * 2f, 
            tooltipTextComponent.preferredHeight + textPaddingSize * 2f);
        _tooltipGameObject.transform.Find("background").GetComponent<RectTransform>().sizeDelta = backgroundSize;
        
        _tooltipGameObject.transform.SetAsLastSibling();
    }
    
    /*
     * Funcion que oculta la tooltip que se estaba mostrindo
     */
    public void HideTooltip()
    {
        _tooltipGameObject.SetActive(false);
    }
    #endregion

    #region Inc/Dec Displayed Data Point
    
    /*
     * Aumenta la cantidad de valores que el grafico muestra.
     */
    public void IncreaseVisibleAmount()
    {
        if (_cachedMaxVisibleAmount < 0) _cachedMaxVisibleAmount = 0;
        ShowGraph(_cachedValueList, _cachedGraphVisual, _cachedMaxVisibleAmount + 1, _cachedGetAxisLabelX, _cachedGetAxisLabelY);
    }
    
    /*
     * Decrementa la cantidad de valores que el grafico muestra.
     */
    public void DecreaseVisibleAmount()
    {
        if (_cachedMaxVisibleAmount <= 0) return;
        ShowGraph(_cachedValueList, _cachedGraphVisual, _cachedMaxVisibleAmount - 1, _cachedGetAxisLabelX, _cachedGetAxisLabelY);
    }
    #endregion
    
    #region Set Axis Labels
    /*
     * Permite cambiar la funcion que definie a la etiqueta que se muestra en el eje X y vuelve a
     * dibujar el ultimo grafico dibujado usando esa funcion.
     */
    public void SetGetAxisLabelX(Func<float, string> getAxisLabelX)
    {
        ShowGraph(_cachedValueList, _cachedGraphVisual, _cachedMaxVisibleAmount, getAxisLabelX, _cachedGetAxisLabelY);
    }
    
    /*
     * Permite cambiar la funcion que definie a la etiqueta que se muestra en el eje Y y vuelve a
     * dibujar el ultimo grafico dibujado usando esa funcion.
     */
    public void SetGetAxisLabelY(Func<float, string> getAxisLabelY)
    {
        ShowGraph(_cachedValueList, _cachedGraphVisual, _cachedMaxVisibleAmount, _cachedGetAxisLabelX, getAxisLabelY);
    }
    #endregion
    
    #region Display Graph
    
    /*
     * Permite cambiar el tipo de grafico, ejemplo pasar de grafico de lineas a grafico de barras. Vuelve a dibujar el
     * grafico segun el tipo definido
     */
    public void SetGraphVisual(IGraphVisual graphVisual)
    {
        ShowGraph(_cachedValueList, graphVisual, _cachedMaxVisibleAmount, _cachedGetAxisLabelX, _cachedGetAxisLabelY);
    }
    
    /*
     * Muestra una lista de valores como un grafico. Recibe como parametros la lista de valores a graficar "valueList",
     * el tipo de grafico que se desea mostrar "graphVisual",
     * la cantidad de elementos maxima a graficar "maxVisibleValueAmount,
     * las funciones que definen que etiquetas mostrar en los ejes X e Y "getAxisLabelX/Y"
     */
    public void ShowGraph(List<float> valueList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1, 
        Func<float, string> getAxisLabelX=null, Func<float, string> getAxisLabelY=null)
    {

        //resetear lista con los objetos previamente instanciados para limpiar un grafico viejo, si lo hay
        foreach (var g in _gameObjects)
        {
            Destroy(g);
        }
        _gameObjects.Clear();
        
        //resettear GraphVisual en caso de que queden variables asignadas por otros graficos
        graphVisual.Reset();

        //limpiar la lista de los objetos visuales de graficos viejos.
        foreach (var graphVisualObject in _graphVisualObjectList)
        {
            graphVisualObject.CleanUp();
        }
        _graphVisualObjectList.Clear();
        
        _yLabelList.Clear();
        
        //validar parametros
        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate(float f) { return f.ToString("n2");};
        }
        
        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate(float f) { return f.ToString("n2");};
        }

        if (maxVisibleValueAmount <= 0 || maxVisibleValueAmount > valueList.Count)
        {
            maxVisibleValueAmount = valueList.Count;
        }
        
        //cachear variables del grafico dibujado
        _cachedValueList = valueList;
        _cachedGraphVisual = graphVisual;
        _cachedGetAxisLabelX = getAxisLabelX;
        _cachedGetAxisLabelY = getAxisLabelY;
        _cachedMaxVisibleAmount = maxVisibleValueAmount;
        
        /*
         * Conseguimos los limites del grafico. Buscamos los valores maximos en el grafico para determinar en que numeros
         * comenzar la escala. Se busca que el eje Y comienze un poco "mas abajo" del valor minimo y termine un poco
         * "mas arriba" que el valor maximo, para que se vea bien.
         */
        var sizeDelta = GraphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float graphWidth = sizeDelta.x; 
        
        float xSize = graphWidth / (maxVisibleValueAmount+1);
        _cachedXSize = xSize;

        float yMinimum, yMaximum;
        
        CalculateYScale(out yMinimum, out yMaximum);
        
        //primero instanciamos los separadores asi quedan atras del grafico
        int yAxisSeparatorCount = 10; //la cantidad de etiquetas y separadores en el eje Y
        for (int i = 0; i <= yAxisSeparatorCount; i++)
        {
            //instanciar etiqueta en el eje Y
            RectTransform labelY = Instantiate(_labelTemplateY, GraphContainer, false);
            labelY.gameObject.SetActive(true);
            
            //que tan arriba en el eje Y va a estar porcentualmente
            float normalizedValue = i * 1f / yAxisSeparatorCount; 
            labelY.anchoredPosition = new Vector2(-5f, normalizedValue * graphHeight);
            
            //poner el texto definido por la funcion getAxisLabel en la etiqueta. El parametro enviado es el valor en Y
            //por el que pasa el separador.
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum-yMinimum)));
            
            _yLabelList.Add(labelY);
            _gameObjects.Add(labelY.gameObject);
            
            //Instanciar separador en el eje Y segun la plantilla
            RectTransform dashY = Instantiate(_dashTemplateY, GraphContainer, false);
            GameObject o;
            (o = dashY.gameObject).SetActive(true);
            dashY.anchoredPosition = new Vector2(_dashTemplateY.anchoredPosition.x, normalizedValue * graphHeight);
            
            _gameObjects.Add(o);
        }

        
        //Para cada valor de la lista, considerando la cantidad maxima a mostrar...
        int xIndex = 0;
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            //en que posicion del grafico se grafica el valor?
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            
            //instanciar etiquta del valor en el eje X
            RectTransform labelX = Instantiate(_labelTemplateX, GraphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -5f);
            labelX.GetComponent<Text>().text = getAxisLabelX(i);
            _gameObjects.Add(labelX.gameObject);
            
            //Instanciar separador del valor en el eje X
            RectTransform dashX = Instantiate(_dashTemplateX, GraphContainer, false);
            GameObject o;
            (o = dashX.gameObject).SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, _dashTemplateX.anchoredPosition.y);
            _gameObjects.Add(o);

            //instanciar objeto visual del grafico (ej: barra o punto.) y configurar su tooltip. 
            string tooltipText = getAxisLabelY(valueList[i]);
            
            _graphVisualObjectList.Add(graphVisual.CreateGraphVisualObject(
                new Vector2(xPosition, yPosition), xSize, tooltipText)
            ); //tambien agregar el objeto visual a la lista de objetos visuales.

            xIndex++;
        }
        
    }

    public void ShowGraph(List<float> valueList, List<float> xAxisLabelList, IGraphVisual graphVisual, int maxVisibleValueAmount = -1, 
        Func<float, string> getAxisLabelX=null, Func<float, string> getAxisLabelY=null)
    {
        if (xAxisLabelList.Count != valueList.Count)
        {
            Debug.LogError("xAxisLabelList debe tener la misma cantidad de elementos que valueList");
            return;
        }

        //resetear lista con los objetos previamente instanciados para limpiar un grafico viejo, si lo hay
        foreach (var g in _gameObjects)
        {
            Destroy(g);
        }
        _gameObjects.Clear();
        
        //resettear GraphVisual en caso de que queden variables asignadas por otros graficos
        graphVisual.Reset();

        //limpiar la lista de los objetos visuales de graficos viejos.
        foreach (var graphVisualObject in _graphVisualObjectList)
        {
            graphVisualObject.CleanUp();
        }
        _graphVisualObjectList.Clear();
        
        _yLabelList.Clear();
        
        //validar parametros
        if (getAxisLabelX == null)
        {
            getAxisLabelX = delegate(float f) { return f.ToString("n2");};
        }
        
        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate(float f) { return f.ToString("n2");};
        }

        if (maxVisibleValueAmount <= 0 || maxVisibleValueAmount > valueList.Count)
        {
            maxVisibleValueAmount = valueList.Count;
        }
        
        //cachear variables del grafico dibujado
        _cachedValueList = valueList;
        _cachedGraphVisual = graphVisual;
        _cachedGetAxisLabelX = getAxisLabelX;
        _cachedGetAxisLabelY = getAxisLabelY;
        _cachedMaxVisibleAmount = maxVisibleValueAmount;
        _cachedXAxisLabelList = xAxisLabelList;
        
        
        /*
         * Conseguimos los limites del grafico. Buscamos los valores maximos en el grafico para determinar en que numeros
         * comenzar la escala. Se busca que el eje Y comienze un poco "mas abajo" del valor minimo y termine un poco
         * "mas arriba" que el valor maximo, para que se vea bien.
         */
        var sizeDelta = GraphContainer.sizeDelta;
        float graphHeight = sizeDelta.y;
        float graphWidth = sizeDelta.x; 
        
        float xSize = graphWidth / (maxVisibleValueAmount+1);
        _cachedXSize = xSize;

        float yMinimum, yMaximum;
        
        CalculateYScale(out yMinimum, out yMaximum);
        
        //primero instanciamos los separadores asi quedan atras del grafico
        int yAxisSeparatorCount = 20; //la cantidad de etiquetas y separadores en el eje Y
        for (int i = 0; i <= yAxisSeparatorCount; i++)
        {
            //instanciar etiqueta en el eje Y
            RectTransform labelY = Instantiate(_labelTemplateY, GraphContainer, false);
            labelY.gameObject.SetActive(true);
            
            //que tan arriba en el eje Y va a estar porcentualmente
            float normalizedValue = i * 1f / yAxisSeparatorCount; 
            labelY.anchoredPosition = new Vector2(-5f, normalizedValue * graphHeight);
            
            //poner el texto definido por la funcion getAxisLabel en la etiqueta. El parametro enviado es el valor en Y
            //por el que pasa el separador.
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum-yMinimum)));
            
            _yLabelList.Add(labelY);
            _gameObjects.Add(labelY.gameObject);
            
            //Instanciar separador en el eje Y segun la plantilla
            RectTransform dashY = Instantiate(_dashTemplateY, GraphContainer, false);
            GameObject o;
            (o = dashY.gameObject).SetActive(true);
            dashY.anchoredPosition = new Vector2(_dashTemplateY.anchoredPosition.x, normalizedValue * graphHeight);
            
            _gameObjects.Add(o);
        }

        int xAxisSeparatorCountAux = Math.Min(20, xAxisLabelList.Count);
        int separateXAxisEvery = (int) Math.Ceiling(((float)xAxisLabelList.Count / (float)xAxisSeparatorCountAux));
        int pointsUntilNextSeparator = 0;
        //Para cada valor de la lista, considerando la cantidad maxima a mostrar...
        int xIndex = 0;
        for (int i = Mathf.Max(valueList.Count - maxVisibleValueAmount, 0); i < valueList.Count; i++)
        {
            //en que posicion del grafico se grafica el valor?
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            
            if (pointsUntilNextSeparator == 0)
            {
                //instanciar etiquta del valor en el eje X
                RectTransform labelX = Instantiate(_labelTemplateX, GraphContainer, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(xPosition, -5f);
                labelX.GetComponent<Text>().text = getAxisLabelX(xAxisLabelList[i]);
                _gameObjects.Add(labelX.gameObject);

                //Instanciar separador del valor en el eje X
                RectTransform dashX = Instantiate(_dashTemplateX, GraphContainer, false);
                GameObject o;
                (o = dashX.gameObject).SetActive(true);
                dashX.anchoredPosition = new Vector2(xPosition, _dashTemplateX.anchoredPosition.y);
                _gameObjects.Add(o);

                pointsUntilNextSeparator = separateXAxisEvery;
            }
    
            //instanciar objeto visual del grafico (ej: barra o punto.) y configurar su tooltip. 
            string tooltipText = "(" + getAxisLabelX(xAxisLabelList[i]) + ", " + getAxisLabelY(valueList[i]) + ")";
            
            _graphVisualObjectList.Add(graphVisual.CreateGraphVisualObject(
                new Vector2(xPosition, yPosition), xSize, tooltipText)
            ); //tambien agregar el objeto visual a la lista de objetos visuales.

            xIndex++;
            pointsUntilNextSeparator--;
        }
    }
    
    /*
     * Actualiza el valor en el indice "index" de la lista de valores con un nuevo valor "value"
     */
    public void UpdateValue(int index, int value)
    {

        if (index < 0 || index > _cachedValueList.Count)
        {
            Debug.LogError("UpdateValue: indice invalido (negativo, o mayor a la cantidad de objetos graficados");
            return;
        }
        //determinamos la escala previa a la modificacion
        float yMinimumBefore, yMaximumBefore;
        CalculateYScale(out yMinimumBefore, out yMaximumBefore);
        
        _cachedValueList[index] = value;
        
        float graphHeight = GraphContainer.sizeDelta.y;
        //float graphWidth = _graphContainer.sizeDelta.x;

        //calculamos la escala luego de la modificacion
        float yMinimum, yMaximum;
        CalculateYScale(out yMinimum, out yMaximum);

        float TOLERANCE = 0.01f;
        bool yScaleChange = Math.Abs(yMaximumBefore - yMaximum) > TOLERANCE || Math.Abs(yMinimumBefore - yMinimum) > TOLERANCE;

        
        if (!yScaleChange)
            //no cambia la escala, solo actualizamos un objeto visual del grafico
        {
            float xPosition = _cachedXSize + index * _cachedXSize;
            float yPosition = ((value - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

            string tooltipText = _cachedGetAxisLabelY(value);
        
            _graphVisualObjectList[index].SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), _cachedXSize, tooltipText);
        }
        else 
            //la escala cambia, tenemos que actualizar todos los objetos y etiquetas para llevarlos a la nueva escala
        {
            int xIndex = 0;
            //actualizar objetos
            for (int i = Mathf.Max(_cachedValueList.Count - _cachedMaxVisibleAmount, 0);
                i < _cachedValueList.Count;
                i++)
            {
                float xPosition = _cachedXSize + xIndex * _cachedXSize;
                float yPosition = ((_cachedValueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;

                string tooltipText = _cachedGetAxisLabelY(_cachedValueList[i]);
                _graphVisualObjectList[xIndex]
                    .SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), _cachedXSize, tooltipText);

                xIndex++;
            }
            //actualizar etiquetas (solo en el eje Y, ya que no se agrega un nuevo valor al grafico, solo se actualiza.
            for (int i = 0; i < _yLabelList.Count; i++)
            {
                float normalizedValue = i * 1f / _yLabelList.Count;
                _yLabelList[i].GetComponent<Text>().text =
                    _cachedGetAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            }
        }
    }

    /*
     * Calcula la escala (valor minimo y maximo) del eje Y del grafico segun los valores a graficar.
     */
    private void CalculateYScale(out float yMinimum, out float yMaximum)
    {
        if (_cachedValueList.Count == 0)
        {
            yMaximum = yMinimum = 0;
            return;
        }
        yMaximum = _cachedValueList[0];
        yMinimum = _cachedValueList[0];
        
        for (int i = Mathf.Max(_cachedValueList.Count - _cachedMaxVisibleAmount, 0); i < _cachedValueList.Count; i++)
        {
            if (_cachedValueList[i] > yMaximum)
            {
                yMaximum = _cachedValueList[i];
            }

            if (_cachedValueList[i] < yMinimum)
            {
                yMinimum = _cachedValueList[i];
            }
        }

        float yDifference = yMaximum - yMinimum;
        if (yDifference <= 0)
            //para que si la diferencia es 0 haya un poco de espacio entre los objetos visuales y los bordes del grafico
        {
            yDifference = 5f;
        }

        yMaximum = yMaximum + (yDifference * 0.2f);
        yMinimum = yMinimum - (yDifference * 0.2f);

        if (_startYScaleAtZero)
            //empezar el grafico en y=0, independientemente del valor minimo en la lista
        {
            yMinimum = 0f; 
        }
    }

    private void CalculateXScale(out float xMin, out float xMax)
    {
        xMax = _cachedXAxisLabelList[0];
        xMin = _cachedXAxisLabelList[0];
        
        for (int i = Mathf.Max(_cachedXAxisLabelList.Count - _cachedMaxVisibleAmount, 0); i < _cachedXAxisLabelList.Count; i++)
        {
            if (_cachedXAxisLabelList[i] > xMax)
            {
                xMax = _cachedXAxisLabelList[i];
            }
            if (_cachedXAxisLabelList[i] < xMin)
            {
                xMin = _cachedXAxisLabelList[i];
            }
        }

        if (_startYScaleAtZero)
            //empezar el grafico en y=0, independientemente del valor minimo en la lista
        {
            xMin = 0f; 
        }
        
    }
    #endregion
    
    /*
     * Interfaz para los tipos de graficos (ej: de barra, de linea...)
     */
    public interface IGraphVisual
    {
        public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText);

        public void Reset(); //por si tenemos que usarla mas de una vez y los atributos deben resetearse (por ejemplo,
        //para reutilizarla un LineGraphVisual hay que poner el _lastDot en null. 
    }
    
    /*
     * Interfaz para los objetos visuales de un tipo de grafico, como barras y puntos.
     */
    public interface IGraphVisualObject
    {
        /*
         * cambiar los datos en base a los cuales se dibuja el elemento visual.
         * "graphPosition" coordenadas X,Y del objeto visual,
         * "graphPositionWidth" que tan ancho se quiere el objeto visual
         * "tooltipText" el texto que mostrara la tooltip al poner el mouse sobre el objeto visual
         */
        void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
        
        /*
         * Elimina el objeto visual
         */
        void CleanUp();

    }
    
    #region Bar Chart
    
    /*
     * Clase que define las barras como objeto visual de un grafico de barras
     */
    public class BarChartVisualObject : IGraphVisualObject
    {
        private GameObject _barGameObject;
        private float _barWidthMultiplier;
        private Window_Graph _windowGraph;
        
        public BarChartVisualObject(Window_Graph windowGraph, GameObject barGameObject, float barWidthMultiplier)
        {
            _barGameObject = barGameObject;
            _barWidthMultiplier = barWidthMultiplier;
            _windowGraph = windowGraph;
        }
        
        public void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            RectTransform rectTransform = _barGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0);
            rectTransform.sizeDelta = new Vector2(graphPositionWidth * _barWidthMultiplier, graphPosition.y);

            GraphMouseDetector graphMouseDetector = _barGameObject.GetComponent<GraphMouseDetector>();

            //nos subscribimos a los eventos para mostrar/ocultar tooltip cuando el mouse esta/deja de estar sobre
            //el punto de datos en el grafico
            graphMouseDetector.onMouseOverGraph += () =>
            {
                _windowGraph.ShowTooltip(tooltipText, graphPosition);
            };

            graphMouseDetector.onMouseOverGraphEnd += () =>
            {
                _windowGraph.HideTooltip();
            };
        }

        public void CleanUp()
        {
            Destroy(_barGameObject);
        }
    }

    /*
     * Clase que define un grafico de barras
     */
    private class BarChartVisual : IGraphVisual
    {
        private RectTransform _graphContrainer;
        private Color _barColor;
        private float _barWidthMultiplier;
        
        private Window_Graph _windowGraph; 
        //para poder invocar el ShowTooltip necesitamos una referencia al WG, o hacer
        //una funcion estatica en el WG. Usamos la referencia para que quede clara la dependencia entre estas clases y
        //el WG si queremos que muestren tooltips.

        public BarChartVisual(Window_Graph windowGraph,RectTransform graphContrainer, Color barColor, float barWidthMultiplier)
        {
            _graphContrainer = graphContrainer;
            _barColor = barColor;
            _barWidthMultiplier = barWidthMultiplier;
            _windowGraph = windowGraph;
        }

        public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            GameObject barGameObject = CreateBar(graphPosition, graphPositionWidth);

            BarChartVisualObject barChartVisualObject = new BarChartVisualObject(_windowGraph, barGameObject, _barWidthMultiplier);
            barChartVisualObject.SetGraphVisualObjectInfo(graphPosition, graphPositionWidth, tooltipText);
            
            return barChartVisualObject;
        }
        
        private GameObject CreateBar(Vector2 graphPosition, float barWidth)
        {
            GameObject gO = new GameObject("bar", typeof(Image));
            gO.transform.SetParent(_graphContrainer, false);
            gO.GetComponent<Image>().color = _barColor;
            RectTransform rectTransform = gO.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(graphPosition.x, 0);
            rectTransform.sizeDelta = new Vector2(barWidth * _barWidthMultiplier, graphPosition.y);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.pivot = new Vector2(.5f, 0f);//ponemos el pivote en el centro y la parte de abajo de la barra
            //para que se expanda desde ahi y no desde el medio

            gO.AddComponent<GraphMouseDetector>();

            return gO;
        }

        public void Reset()
        {
            return;
        }
    }
    #endregion
    
    #region Line Graph

    /*
     * Clase que define los puntos y lineas como elementos de un grafico de lineas
     */
    public class LineGraphVisualObject :IGraphVisualObject
    {
        //Usamos un evento para avisar que cambio de lugar el punto en el grafico y que se tienen que rearmar las conexiones
        public event OnChangedGraphVisualObjectInfo onChangedGraphVisualObjectInfo;
        public delegate void OnChangedGraphVisualObjectInfo();
        
        private GameObject _dotGameObject;
        private GameObject _dotConnectionGameObject;
        private LineGraphVisualObject _lastVisualObject;
        private Window_Graph _windowGraph;
        
        public LineGraphVisualObject(Window_Graph windowGraph ,GameObject dotGameObject,
            GameObject dotConnectionGameObject, LineGraphVisualObject lastVisualObject)
        {
            _dotGameObject = dotGameObject;
            _dotConnectionGameObject = dotConnectionGameObject;
            _lastVisualObject = lastVisualObject;
            _windowGraph = windowGraph;

            if (_lastVisualObject != null)
            {
                _lastVisualObject.onChangedGraphVisualObjectInfo += UpdateDotConnection;
            }
        }

        public void SetGraphVisualObjectInfo(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            RectTransform rectTransform = _dotGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = graphPosition;
            
            UpdateDotConnection();

            GraphMouseDetector graphMouseDetector = _dotGameObject.GetComponent<GraphMouseDetector>();
            
            graphMouseDetector.onMouseOverGraph += () =>
            {
                _windowGraph.ShowTooltip(tooltipText, graphPosition);
            };

            graphMouseDetector.onMouseOverGraphEnd += () =>
            {
                _windowGraph.HideTooltip();
            };

            onChangedGraphVisualObjectInfo?.Invoke();
        }

        public void CleanUp()
        {
            Destroy(_dotGameObject);
            Destroy(_dotConnectionGameObject);
        }

        public Vector2 GetGraphPosition()
        {
            RectTransform rectTransform = _dotGameObject.GetComponent<RectTransform>();
            return rectTransform.anchoredPosition;
        }

        private void UpdateDotConnection()
        {
            if (_dotConnectionGameObject != null)
            {
                RectTransform dotConnectionRectTransform = _dotConnectionGameObject.GetComponent<RectTransform>();
                Vector2 dir = (_lastVisualObject.GetGraphPosition() - GetGraphPosition()).normalized;
                float distance = Vector2.Distance(GetGraphPosition(), _lastVisualObject.GetGraphPosition());
                dotConnectionRectTransform.anchorMin = new Vector2(0, 0);
                dotConnectionRectTransform.anchorMax = new Vector2(0, 0);
                dotConnectionRectTransform.sizeDelta = new Vector2(distance, 3);
                dotConnectionRectTransform.anchoredPosition = GetGraphPosition() + dir * distance * .5f;
                //toma el vector dir y lo convierte en un angulo de 0 a 360
                dotConnectionRectTransform.localEulerAngles = new Vector3(0, 0, (Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI));
            }
        }
    }
    
    /*
     * clase que define un grafico de lineas
     */
    public class LineGraphVisual : IGraphVisual
    {
        private RectTransform _graphContainer;
        private Sprite _dotSprite;
        private LineGraphVisualObject _lastLineGraphVisualObject;
        private Color _dotColor;
        private Color _connectionColor;
        private Window_Graph _windowGraph;

        public LineGraphVisual(Window_Graph windowGraph,RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color connectionColor)
        {
            _graphContainer = graphContainer;
            _dotSprite = dotSprite;
            _dotColor = dotColor;
            _connectionColor = connectionColor;
            _windowGraph = windowGraph;
            _lastLineGraphVisualObject = null;
        }

        public IGraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, float graphPositionWidth, string tooltipText)
        {
            GameObject dotGameObject = CreateDot(graphPosition);

            GameObject dotConnectionGameObject = null;
            if (_lastLineGraphVisualObject != null)
            {
                dotConnectionGameObject = CreateDotConnection(_lastLineGraphVisualObject.GetGraphPosition(),
                    dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                
                //ponemos el conector atras de todo excepto el background
                //de esta manera no tapa al punto y no complica conseguir la tooltip del punto.
                int backgroundIndex = _graphContainer.transform.Find("background").GetSiblingIndex();
                dotConnectionGameObject.transform.SetSiblingIndex(backgroundIndex + 1);
                
            }

            LineGraphVisualObject lineGraphVisualObject = new LineGraphVisualObject(_windowGraph, dotGameObject,
                dotConnectionGameObject, _lastLineGraphVisualObject);
            lineGraphVisualObject.SetGraphVisualObjectInfo(graphPosition, graphPositionWidth, tooltipText);

            _lastLineGraphVisualObject = lineGraphVisualObject;

            return lineGraphVisualObject;
        }
        
        GameObject CreateDot(Vector2 anchoredPosition)
        {
            GameObject gO = new GameObject("dot", typeof(Image));
            gO.transform.SetParent(_graphContainer, false);
            gO.GetComponent<Image>().sprite = _dotSprite;
            gO.GetComponent<Image>().color = _dotColor;
            RectTransform rectTransform = gO.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = new Vector2(11, 11);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.anchorMin = new Vector2(0, 0);
            
            GraphMouseDetector graphMouseDetector = gO.AddComponent<GraphMouseDetector>();
            return gO;
        }

        GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gO = new GameObject("dotConnection", typeof(Image));
            gO.transform.SetParent(_graphContainer,false);
            gO.GetComponent<Image>().color = _connectionColor;
            RectTransform rectTransform = gO.GetComponent<RectTransform>();
            Vector2 dir = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, 3);
            rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
            //toma el vector dir y lo convierte en un angulo de 0 a 360
            rectTransform.localEulerAngles = new Vector3(0, 0, (Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI));
            return gO;
        }

        public void Reset()
        {
            _lastLineGraphVisualObject = null;
        }
    }
    #endregion
    
    /*
     * Clase encargada de capturar eventos del mouse sobre un elemento de la interfaz Unity para que clases puedan
     * escucharlos
     */
    private class GraphMouseDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event OnMouseOverGraph onMouseOverGraph;
        public delegate void OnMouseOverGraph();

        public event OnMouseOverGraphEnd onMouseOverGraphEnd;

        public delegate void OnMouseOverGraphEnd();
        public void OnPointerEnter(PointerEventData eventData)
        {
            onMouseOverGraph?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onMouseOverGraphEnd?.Invoke();
        }
    }
}
