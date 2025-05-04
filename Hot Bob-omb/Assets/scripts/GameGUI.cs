using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameGUI : MonoBehaviour
{
    [Header("Referencias Principales")]
    public GameManager gameManager;
    public GameObject bomba;

    [Header("Configuración de Textos")]
    public TMP_FontAsset fontGeneral;
    public TMP_FontAsset fontNombres;
    public TMP_FontAsset fontFase;
    public string[] nombresPersonajes = { "Jugador 1", "Jugador 2", "Jugador 3" };

    [Header("Configuración de Tiempos")]
    public float tiempoFaseNormal = 60f;
    public float tiempoFaseFinal = 30f;

    [Header("Configuración Visual")]
    public Color colorNormal = Color.white;
    public Color colorAlerta = Color.yellow;
    public Color colorPeligro = Color.red;
    [Range(1f, 5f)] public float tamańoFuenteNombres = 3f;
    [Range(0.5f, 3f)] public float alturaNombres = 1.5f;
    public Color colorNombreNormal = Color.white;
    public Color colorNombrePortador = Color.red;

    [Header("Posiciones")]
    public Vector2 posicionFase = new Vector2(50f, -50f);
    public Vector2 posicionTiempo = new Vector2(-50f, -50f);
    public Vector2 posicionNombres = new Vector2(20f, 20f);
    [Range(0.1f, 2f)] public float grosorFuenteFase = 1f;

    [Header("Botón de Volver")]
    public Sprite spriteBoton; // Arrastra tu PNG aquí
    public Vector2 posicionBoton = new Vector2(0, -150);
    public Vector2 tamanoBoton = new Vector2(200, 100);
    [Range(0.8f, 1f)] public float escalaAlPresionar = 0.95f;

    private TextMeshProUGUI timerText;
    private TextMeshProUGUI phaseText;
    private TextMeshProUGUI winnerText;
    private List<TextMeshProUGUI> nombresUI = new List<TextMeshProUGUI>();
    private List<GameObject> nombresFlotantes = new List<GameObject>();
    private float tiempoRestanteFase;
    private bool faseFinal = false;
    private float ultimaActualizacionFase;
    private GameObject botonVolver;

    void Start()
    {
        InicializarUI();
        ConfigurarNombresPersonajes();
        tiempoRestanteFase = tiempoFaseNormal;
    }

    void InicializarUI()
    {
        if (FindObjectOfType<Canvas>() == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        phaseText = CrearTextoUI("PhaseText", "FASE NORMAL",
                               posicionFase,
                               28, colorAlerta,
                               TextAlignmentOptions.Left,
                               TextAnchor.UpperLeft,
                               fontFase != null ? fontFase : fontGeneral);

        if (fontFase != null)
        {
            phaseText.fontWeight = FontWeight.Bold;
            phaseText.font = fontFase;
        }

        timerText = CrearTextoUI("TimerText", tiempoFaseNormal.ToString("F1"),
                               posicionTiempo,
                               36, colorNormal,
                               TextAlignmentOptions.Right,
                               TextAnchor.UpperRight);

        winnerText = CrearTextoUI("WinnerText", "", Vector2.zero, 48, Color.green,
                               TextAlignmentOptions.Center,
                               TextAnchor.MiddleCenter);
        winnerText.enableWordWrapping = false;
        winnerText.gameObject.SetActive(false);
    }

    TextMeshProUGUI CrearTextoUI(string nombre, string texto, Vector2 offset, int tamańo, Color color,
                               TextAlignmentOptions alineacion, TextAnchor ancla, TMP_FontAsset fuente = null)
    {
        GameObject textGO = new GameObject(nombre);
        textGO.transform.SetParent(FindObjectOfType<Canvas>().transform);

        RectTransform rt = textGO.AddComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(
            ancla == TextAnchor.UpperLeft || ancla == TextAnchor.MiddleLeft || ancla == TextAnchor.LowerLeft ? 0 :
            ancla == TextAnchor.UpperCenter || ancla == TextAnchor.MiddleCenter || ancla == TextAnchor.LowerCenter ? 0.5f : 1f,
            ancla == TextAnchor.LowerLeft || ancla == TextAnchor.LowerCenter || ancla == TextAnchor.LowerRight ? 0 :
            ancla == TextAnchor.MiddleLeft || ancla == TextAnchor.MiddleCenter || ancla == TextAnchor.MiddleRight ? 0.5f : 1f
        );
        rt.anchoredPosition = offset;

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = texto;
        tmp.fontSize = tamańo;
        tmp.color = color;
        tmp.alignment = alineacion;
        tmp.font = fuente != null ? fuente : fontGeneral;
        tmp.enableWordWrapping = false;

        return tmp;
    }

    void ConfigurarNombresPersonajes()
    {
        GameObject panelNombres = new GameObject("PanelNombres");
        panelNombres.transform.SetParent(FindObjectOfType<Canvas>().transform);

        RectTransform rt = panelNombres.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(0, 0);
        rt.pivot = new Vector2(0, 0);
        rt.anchoredPosition = posicionNombres;

        VerticalLayoutGroup layout = panelNombres.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.LowerLeft;
        layout.spacing = 10f;
        layout.padding = new RectOffset(10, 10, 10, 10);

        if (gameManager != null && gameManager.personajes != null)
        {
            for (int i = 0; i < gameManager.personajes.Length; i++)
            {
                if (gameManager.personajes[i] != null)
                {
                    GameObject nombreUI = new GameObject("NombreUI_" + i);
                    nombreUI.transform.SetParent(panelNombres.transform);

                    TextMeshProUGUI tmpUI = nombreUI.AddComponent<TextMeshProUGUI>();
                    tmpUI.text = i < nombresPersonajes.Length ? nombresPersonajes[i] : "Jugador " + (i + 1);
                    tmpUI.fontSize = 24;
                    tmpUI.alignment = TextAlignmentOptions.Left;
                    tmpUI.font = fontNombres;
                    tmpUI.enableWordWrapping = false;

                    nombresUI.Add(tmpUI);

                    GameObject nombreFlotante = new GameObject("NombreFlotante_" + i);
                    nombreFlotante.transform.SetParent(gameManager.personajes[i].transform);
                    nombreFlotante.transform.localPosition = new Vector3(0, alturaNombres, 0);

                    TextMeshPro tmp = nombreFlotante.AddComponent<TextMeshPro>();
                    tmp.text = i < nombresPersonajes.Length ? nombresPersonajes[i] : "Jugador " + (i + 1);
                    tmp.fontSize = tamańoFuenteNombres;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.font = fontNombres;
                    tmp.color = colorNombreNormal;

                    nombresFlotantes.Add(nombreFlotante);
                }
            }
        }
    }

    void Update()
    {
        if (gameManager == null || bomba == null) return;

        ActualizarTemporizador();
        ActualizarFase();
        ActualizarNombres();
        ActualizarEstadoPersonajes();
    }

    void ActualizarTemporizador()
    {
        GameObject portador = ObtenerPortadorBomba();
        if (portador != null)
        {
            tiempoRestanteFase -= Time.deltaTime;
        }

        timerText.text = Mathf.Max(tiempoRestanteFase, 0f).ToString("F1");
        timerText.color = tiempoRestanteFase <= 10f ? colorPeligro :
                         (tiempoRestanteFase <= 20f ? colorAlerta : colorNormal);

        if (tiempoRestanteFase <= 0f)
        {
            tiempoRestanteFase = 0f;
        }
    }

    void ActualizarFase()
    {
        if (Time.time - ultimaActualizacionFase > 1f)
        {
            int activos = ContarPersonajesActivos();
            bool nuevaFaseFinal = activos <= 2;

            if (nuevaFaseFinal != faseFinal)
            {
                faseFinal = nuevaFaseFinal;
                tiempoRestanteFase = faseFinal ? tiempoFaseFinal : tiempoFaseNormal;
                ultimaActualizacionFase = Time.time;

                phaseText.text = "FASE " + (faseFinal ? "FINAL" : "NORMAL");
                phaseText.color = faseFinal ? colorPeligro : colorAlerta;

                if (fontFase != null)
                {
                    phaseText.fontWeight = FontWeight.Bold;
                    phaseText.font = fontFase;
                }
            }
        }
    }

    void ActualizarNombres()
    {
        GameObject portador = ObtenerPortadorBomba();

        for (int i = 0; i < nombresUI.Count; i++)
        {
            if (i < gameManager.personajes.Length && gameManager.personajes[i] != null)
            {
                bool esPortador = (gameManager.personajes[i] == portador);
                nombresUI[i].color = esPortador ? colorNombrePortador : colorNombreNormal;
                nombresUI[i].text = (i < nombresPersonajes.Length ? nombresPersonajes[i] : "Jugador " + (i + 1)) +
                                    (esPortador ? " (BOMBA)" : "");

                if (i < nombresFlotantes.Count && nombresFlotantes[i] != null)
                {
                    TextMeshPro tmp = nombresFlotantes[i].GetComponent<TextMeshPro>();
                    tmp.color = esPortador ? colorNombrePortador : colorNombreNormal;
                }
            }
        }
    }

    void ActualizarEstadoPersonajes()
    {
        for (int i = 0; i < nombresUI.Count; i++)
        {
            if (i < gameManager.personajes.Length && gameManager.personajes[i] != null)
            {
                bool activo = gameManager.personajes[i].activeSelf;
                nombresUI[i].gameObject.SetActive(activo);
                if (i < nombresFlotantes.Count && nombresFlotantes[i] != null)
                {
                    nombresFlotantes[i].SetActive(activo);
                }
            }
        }
    }

    int ContarPersonajesActivos()
    {
        int count = 0;
        if (gameManager != null && gameManager.personajes != null)
        {
            foreach (var personaje in gameManager.personajes)
            {
                if (personaje != null && personaje.activeSelf) count++;
            }
        }
        return count;
    }

    GameObject ObtenerPortadorBomba()
    {
        GameObject masCercano = null;
        float minDistancia = float.MaxValue;

        if (gameManager != null && gameManager.personajes != null && bomba != null)
        {
            foreach (var personaje in gameManager.personajes)
            {
                if (personaje != null && personaje.activeSelf)
                {
                    float distancia = Vector3.Distance(bomba.transform.position, personaje.transform.position);
                    if (distancia < minDistancia)
                    {
                        minDistancia = distancia;
                        masCercano = personaje;
                    }
                }
            }
        }

        return masCercano;
    }

    public void MostrarGanador()
    {
        GameObject ganador = ObtenerGanador();
        if (ganador != null)
        {
            int index = System.Array.IndexOf(gameManager.personajes, ganador);
            string nombreGanador = index < nombresPersonajes.Length ? nombresPersonajes[index] : ganador.name;
            winnerText.text = $"¡{nombreGanador.ToUpper()} GANA!";
        }
        else
        {
            winnerText.text = "¡EMPATE!";
        }
        winnerText.gameObject.SetActive(true);

        if (botonVolver == null)
        {
            CrearBotonVolver();
        }
        else
        {
            botonVolver.SetActive(true);
        }
    }

    void CrearBotonVolver()
    {
        // Crear objeto del botón
        botonVolver = new GameObject("BotonVolver");
        botonVolver.transform.SetParent(FindObjectOfType<Canvas>().transform);

        // Configurar RectTransform
        RectTransform rt = botonVolver.AddComponent<RectTransform>();
        rt.sizeDelta = tamanoBoton;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = posicionBoton;

        // Añadir Image
        Image imagenBoton = botonVolver.AddComponent<Image>();
        imagenBoton.sprite = spriteBoton;
        imagenBoton.preserveAspect = true;

        // Añadir Button
        Button boton = botonVolver.AddComponent<Button>();
        boton.targetGraphic = imagenBoton;

        // Configurar transición de color
        ColorBlock colors = boton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f);
        colors.pressedColor = new Color(0.9f, 0.9f, 0.9f);
        colors.fadeDuration = 0.1f;
        boton.colors = colors;

        // Animación al pulsar
        boton.onClick.AddListener(() => {
            StartCoroutine(AnimarBoton());
            VolverAlMenu();
        });
    }

    IEnumerator AnimarBoton()
    {
        RectTransform rt = botonVolver.GetComponent<RectTransform>();
        Vector3 escalaOriginal = rt.localScale;

        // Escalar al presionar
        rt.localScale = escalaOriginal * escalaAlPresionar;
        yield return new WaitForSecondsRealtime(0.1f);
        rt.localScale = escalaOriginal;
    }

    void VolverAlMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("PantallaInicialHotBomb");
    }

    GameObject ObtenerGanador()
    {
        if (gameManager != null && gameManager.personajes != null)
        {
            foreach (var personaje in gameManager.personajes)
            {
                if (personaje != null && personaje.activeSelf) return personaje;
            }
        }
        return null;
    }
}