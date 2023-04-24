using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Debug = UnityEngine.Debug;

public class Bet : MonoBehaviour
{
    StackTrace stackTrace;
    public double dineroDisponible;
    public double dineroApostado;
    public Button allInButton;
    public Button clearButton;
    public Button betButton;
    public Button playAgainButton;
    public Button hitButton;
    public Button hitFalsoButton;
    public Button standButton;

    public Button mas10;
    public Button mas100;
    public Button mas1000;
    public Button menos10;
    public Button menos100;
    public Button menos1000;


    public Text dineroDisponible_text;
    public Text dineroApostado_text;

    private void Start()
    {
        dineroDisponible = 1000;
        dineroApostado = 0;
        activarBotonesApostar();
        desactivarBotonesJugar();
        playAgainButton.interactable = false;

    }

    private void Update()
    {
        dineroDisponible_text.text = dineroDisponible.ToString();
        dineroApostado_text.text = dineroApostado.ToString();
    }

    public void anyadirApuesta(double apuestaValue)
    {
        if (dineroDisponible >= dineroApostado + apuestaValue)
        {
            dineroApostado += apuestaValue;
        }
    }

    public void eliminarApuesta(double eliminarApuestaValue)
    {
        if (dineroApostado >= eliminarApuestaValue)
        {
            dineroApostado -= eliminarApuestaValue;
        }
    }

    public void anyadirApuestaDiez()
    {
        if (dineroDisponible >= dineroApostado + 10) dineroApostado += 10;

    }

    public void anyadirApuestaCien()
    {
        if (dineroDisponible >= dineroApostado + 100) dineroApostado += 100;
    }

    public void anyadirApuestaMil()
    {
        if (dineroDisponible >= dineroApostado + 1000) dineroApostado += 1000;
    }

    public void eliminarApuestaDiez()
    {
        if (dineroApostado >= 10) dineroApostado -= 10;
    }
    public void eliminarApuestaCien()
    {
        if (dineroApostado >= 100) dineroApostado -= 100;
    }
    public void eliminarApuestaMil()
    {
        if (dineroApostado >= 1000) dineroApostado -= 1000;
    }


    public void apostarTodo()
    {
        dineroApostado = dineroDisponible;
    }

    public void borrarApuesta()
    {
        dineroApostado = 0;
    }

    public void apostarAndEmpezar()
    {
        
        //Barajar y empezar juego
        this.gameObject.GetComponent<Deck>().ShuffleCards();
        this.gameObject.GetComponent<Deck>().StartGame();

        // Habilitar botones del Juego
        activarBotonesJugar();
        desactivarBotonesApostar();
        betButton.OnPointerExit(null);
        betButton.interactable = false;

        //Deshabilitar botones de Apuestas        desactivarBotonesApostar();
    }



    public void ganarApuesta()
    {
        dineroDisponible += 2 * dineroApostado;
        dineroApostado = 0;
        desactivarBotonesJugar();
        stackTrace = new StackTrace();
        print("stackTrace !! " + stackTrace.GetFrame(1).GetMethod().Name);

    }

    public void perderApuesta()
    {
        dineroDisponible -= dineroApostado;
        dineroApostado = 0;
        desactivarBotonesJugar();
        stackTrace = new StackTrace();
        print("stackTrace !! " + stackTrace.GetFrame(1).GetMethod().Name);

    }

    public void empatarApuesta()
    {
        dineroApostado = 0;
        desactivarBotonesJugar();
        stackTrace = new StackTrace();
        print("stackTrace !! " + stackTrace.GetFrame(1).GetMethod().Name);
    }
    public void activarBotonesApostar()
    {
        allInButton.interactable = true;
        clearButton.interactable = true;
        betButton.interactable = true;
        mas10.interactable = true;
        mas100.interactable = true;
        mas1000.interactable = true;
        menos10.interactable = true;
        menos100.interactable = true;
        menos1000.interactable = true;
    }

    public void desactivarBotonesApostar()
    {
        allInButton.interactable = false;
        clearButton.interactable = false;
        betButton.interactable = false;
        mas10.interactable = false;
        mas100.interactable = false;
        mas1000.interactable = false;
        menos10.interactable = false;
        menos100.interactable = false;
        menos1000.interactable = false;
    }
    public void activarBotonesJugar()
    {
        Debug.Log("ACTIVATE!!");
        hitButton.gameObject.SetActive(true);
        hitButton.interactable = true;
        hitFalsoButton.gameObject.SetActive(false);
        standButton.interactable = true;
    }

    public void desactivarBotonesJugar()
    {
        standButton.interactable = false;
        hitButton.gameObject.SetActive(false);
        hitButton.interactable = false;
        hitFalsoButton.gameObject.SetActive(true);
        playAgainButton.interactable = true;
    }
    public void ResetCanvas()
    {
        activarBotonesApostar();
        desactivarBotonesJugar();
        playAgainButton.interactable = false;

    }
}