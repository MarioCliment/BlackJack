﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    //Atributes

    //Modelo de las cartas

    [Header("Modelo Carta")]

    public GameObject defaultCarta;
    public Sprite[] faces;
    public int[] values = new int[52];
    public string[] nombres = new string[52];

    int cardIndex = 0;

    //Para el player y el dealer 

    [Header("Player & Dealer")]

    public GameObject dealer;
    public GameObject player;

    //Botones
    [Header("Botones")]

    public Button hitButton;
    public Button standButton;
    public Button playAgainButton;

    //HUD

    [Header("Mensajes")]

    public Text finalMessage;
    public Text probMessage;
    public Text pointPlayer;
    public Text pointDealer;

    //Baraja

    [Header("Barajas")]
    public List<GameObject> BarajaInicial = new List<GameObject>();
    public List<GameObject> BarajaAleatoria = new List<GameObject>();
    public List<GameObject> BarajaProbabilidades = new List<GameObject>();

 
    //Methods 

    private void Awake()
    {
        InitCardValues();
    }


    private void Update()
    {
        pointPlayer.text = "Puntuación: " + player.GetComponent<CardHand>().points.ToString();
    }


    //Aquí se inicializan los valores de las cartas

    private void InitCardValues()
    {
        //  Crear una lista de valores para un palo
        int[] valoresPalo = new int[13];
        int indiceValoresPalo = 0;
        valoresPalo[0] = 11;
        for (int i = 1; i < valoresPalo.Length; i++)
        {
            if (i < 10)
            {
                valoresPalo[i] = i + 1;
            }
            else
            {
                valoresPalo[i] = 10;
            }
        }

        // Asignar los valores a las cartas
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = valoresPalo[indiceValoresPalo];

            indiceValoresPalo++;
            if (indiceValoresPalo == 13)
            {
                indiceValoresPalo = 0;
            }
        }

        // Generar una baraja inicial de objetos GameObject
        for (int i = 0; i < faces.Length; i++)
        {
            GameObject carta = Instantiate(defaultCarta);
            carta.name = nombres[i];
            carta.GetComponent<CardModel>().value = values[i];
            carta.GetComponent<CardModel>().front = faces[i];

            BarajaInicial.Add(carta);
        }

    }

    //Aquí se barajan las cartas

    public void ShuffleCards()
    {
        BarajaAleatoria.Clear();

        //Creación y copia de baraja en una baraja Auxiliar
        List<GameObject> BarajaAux = new List<GameObject>();
        foreach (GameObject carta in BarajaInicial)
        {
            BarajaAux.Add(carta);
        }

        //Baraja aleatoria
        for (int i = 0; i <= 51; i++)
        {
            int indiceRandom = Random.Range(0, BarajaAux.Count - 1);
            GameObject carta = BarajaAux[indiceRandom];
            BarajaAux.RemoveAt(indiceRandom);
            BarajaAleatoria.Add(carta);
        }

        //Baraja para calcular probabilidades
        foreach (GameObject carta in BarajaAleatoria)
        {
            BarajaProbabilidades.Add(carta);
        }
        BarajaProbabilidades.Add(BarajaAleatoria[1]);

    }

    //Aquí se empieza el juego

    public void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();

            //Caso 1: Empatar al empezar partida
            if (dealer.GetComponent<CardHand>().points == 21 && player.GetComponent<CardHand>().points == 21)
            {
                //Mensaje de empate
                finalMessage.text = "¡Empate!";
                finalMessage.color = Color.yellow;

                //Desactivar botones
                hitButton.interactable = false;
                standButton.interactable = false;

                //Mostrar puntuación dealer
                pointDealer.enabled = true;
                pointDealer.text = "Puntuación del dealer: " + dealer.GetComponent<CardHand>().points.ToString();
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

                //Apuesta
                this.gameObject.GetComponent<Bet>().empatarApuesta();
            }

            //Caso 2: Ganar si player tiene 21 y el dealer tiene menos de 21 al empezar partida
            else if (player.GetComponent<CardHand>().points == 21 || dealer.GetComponent<CardHand>().points > 21)
            {
                //Mensaje de victoria
                finalMessage.text = "¡Ganaste!";
                finalMessage.color = Color.green;

                //Desactivar botones
                hitButton.interactable = false;
                standButton.interactable = false;

                //Mostrar puntuación dealer
                pointDealer.enabled = true;
                pointDealer.text = "Puntuación del dealer: " + dealer.GetComponent<CardHand>().points.ToString();
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

                //Apuesta
                this.gameObject.GetComponent<Bet>().ganarApuesta();
            }

            //Caso 3: Perder si dealer tiene 21 y el player tiene menos de 21 al empezar partida
            else if (dealer.GetComponent<CardHand>().points == 21 || player.GetComponent<CardHand>().points > 21)
            {
                //Mensaje de derrota
                finalMessage.text = "¡Perdiste!";
                finalMessage.color = Color.red;

                //Desactivar botones
                hitButton.interactable = false;
                standButton.interactable = false;

                //Mostrar puntuación dealer
                pointDealer.enabled = true;
                pointDealer.text = "Puntuación del dealer: " + dealer.GetComponent<CardHand>().points.ToString();
                dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

                //Apuesta
                this.gameObject.GetComponent<Bet>().perderApuesta();
            }
        }
    }

    //Aquí se calculan las probabilidades

    private void CalculateProbabilities()
    {
        //PROBABILIDAD 1: Probabilidad de que el crupier tenga una puntuación más alta que el jugador con la carta oculta

        double cartasProb1 = 0;
        double Prob1 = 0;
        foreach (GameObject carta in BarajaProbabilidades)
        {
            if (carta.GetComponent<CardModel>().value
                + BarajaAleatoria[3].gameObject.GetComponent<CardModel>().value
                > player.gameObject.GetComponent<CardHand>().points)
            {
                cartasProb1++;
            }
        }
        Prob1 = (cartasProb1 / BarajaProbabilidades.Count) * 100;
        probMessage.text = "El crupier tiene una puntuación más alta: " + string.Format("{0:0.00}", Prob1) + "% \n";

        //PROBABILIDAD 2: Probabilidad de que el jugador obtenga entre 17 y 21 si pide una carta

        double cartasProb2 = 0;
        double Prob2 = 0;
        foreach (GameObject carta in BarajaProbabilidades)
        {
            if (carta.GetComponent<CardModel>().value + player.gameObject.GetComponent<CardHand>().points <= 21
                && carta.GetComponent<CardModel>().value + player.gameObject.GetComponent<CardHand>().points >= 17)
            {
                cartasProb2++;
            }
        }
        Prob2 = (cartasProb2 / BarajaProbabilidades.Count) * 100;
        probMessage.text += "Obtener entre 17 y 21: " + string.Format("{0:0.00}", Prob2) + "% \n";

        //PROBABILIDAD 3: Probabilidad de que el jugador obtenga más de 21 si pide una carta          

        double cartasProb3 = 0;
        double Prob3 = 0;
        foreach (GameObject carta in BarajaProbabilidades)
        {
            if (carta.GetComponent<CardModel>().value + player.gameObject.GetComponent<CardHand>().points > 21)
            {
                cartasProb3++;
            }
        }
        Prob3 = (cartasProb3 / BarajaProbabilidades.Count) * 100;
        probMessage.text += "Obtener más de 21: " + string.Format("{0:0.00}", Prob3) + "%";
    }

    //El dealer hacer el push con la baraja aleatoria

    void PushDealer()
    {
        //Remover la carta de la baraja de probabilidades
        BarajaProbabilidades.Remove(BarajaAleatoria[cardIndex]);

        //Dar la carta al dealer
        dealer.GetComponent<CardHand>().Push(BarajaAleatoria[cardIndex].GetComponent<CardModel>().front,
            BarajaAleatoria[cardIndex].GetComponent<CardModel>().value);

        //Incrementar el índice de la carta
        cardIndex++;
    }

    //El jugador hacer el push con la baraja aleatoria

    void PushPlayer()
    {
        //Remover la carta de la baraja de probabilidades
        BarajaProbabilidades.Remove(BarajaAleatoria[cardIndex]);

        //Dar la carta al player
        player.GetComponent<CardHand>().Push(BarajaAleatoria[cardIndex].GetComponent<CardModel>().front,
           BarajaAleatoria[cardIndex].GetComponent<CardModel>().value);

        //Incrementar el índice de la carta
        cardIndex++;
        //Calcular la probabilidad
        CalculateProbabilities();
    }

    //Función hit

    public void Hit()
    {

        //Repartimos carta al jugador
        PushPlayer();

        //Caso 4: Perder si player tiene mas de 21 al pedir una carta
        if (player.GetComponent<CardHand>().points > 21)
        {
            //Mensaje de derrota
            finalMessage.text = "Has perdido";
            finalMessage.color = Color.red;

            //Desactivar botones
            hitButton.OnPointerExit(null);
            hitButton.interactable = false;
            standButton.interactable = false;

            //Mostrar puntuación dealer
            pointDealer.enabled = true;
            pointDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

            //Apuesta
            this.gameObject.GetComponent<Bet>().perderApuesta();
        }

        //Caso 5: Ganar si player tiene 21 al pedir una carta
        else if (player.GetComponent<CardHand>().points == 21)
        {
            //Mensaje de victoria
            finalMessage.text = "Has ganado";
            finalMessage.color = Color.green;

            //Desactivar botones
            hitButton.OnPointerExit(null);
            hitButton.interactable = false;
            standButton.interactable = false;

            //Mostrar puntuación dealer
            pointDealer.enabled = true;
            pointDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

            //Apuesta
            this.gameObject.GetComponent<Bet>().ganarApuesta();
        }
    }

    //Funcion Stand

    public void Stand()
    {

        //Desactivar botones
        hitButton.interactable = false;
        standButton.interactable = false;

        //Girar 1ª carta del dealer
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);

        //Si tiene menos de 17 el dealer pide cartas
        while (dealer.GetComponent<CardHand>().points < 17)
        {
            PushDealer();
        }

        //Caso 6: Empatar cuando el dealer y player tienen la misma puntuación
        if (dealer.GetComponent<CardHand>().points == player.GetComponent<CardHand>().points)
        {
            //Mensaje de empate
            finalMessage.text = "Empate";
            finalMessage.color = Color.yellow;

            //Desactivar botones
            hitButton.interactable = false;
            standButton.interactable = false;

            //Mostrar puntuación dealer
            pointDealer.enabled = true;
            pointDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();

            //Apuesta
            this.gameObject.GetComponent<Bet>().empatarApuesta();
        }

        //Caso 7: Ganar cuando el dealer se ha pasado de 21
        else if (dealer.GetComponent<CardHand>().points > 21)
        {
            //Mensaje de victoria
            finalMessage.text = "Has ganado";
            finalMessage.color = Color.green;

            //Desactivar botones
            hitButton.interactable = false;
            standButton.interactable = false;

            //Mostrar puntuación dealer
            pointDealer.enabled = true;
            pointDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();

            //Apuesta
            this.gameObject.GetComponent<Bet>().ganarApuesta();
        }

        //Caso 7: Ganar cuando la puntuacion del player es mayor a la del dealer al plantarse
        else if (player.GetComponent<CardHand>().points > dealer.GetComponent<CardHand>().points)
        {
            //Mensaje de victoria
            finalMessage.text = "Has ganado";
            finalMessage.color = Color.green;

            //Desactivar botones
            hitButton.interactable = false;
            standButton.interactable = false;

            //Mostrar puntuación dealer
            pointDealer.enabled = true;
            pointDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();

            //Apuesta
            this.gameObject.GetComponent<Bet>().ganarApuesta();
        }

        //Caso 7:Perder cuando la puntuacion del player es menor a la del dealer al plantarse
        else if (player.GetComponent<CardHand>().points < dealer.GetComponent<CardHand>().points)
        {
            //Mensaje de derrota
            finalMessage.text = "Has perdido";
            finalMessage.color = Color.red;

            //Desactivar botones
            hitButton.interactable = false;
            standButton.interactable = false;

            //Mostrar puntuación dealer
            pointDealer.enabled = true;
            pointDealer.text = "Puntuación: " + dealer.GetComponent<CardHand>().points.ToString();

            //Apuesta
            this.gameObject.GetComponent<Bet>().perderApuesta();
        }
        
        standButton.interactable = false;
        Debug.Log("standButton no interactable");
    }

    //Funcion para volver a jugar

    public void PlayAgain()
    {

        //Reseteos
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        BarajaProbabilidades.Clear();
        probMessage.text = "";
        cardIndex = 0;
        pointDealer.enabled = false;

        //Activar Botones de Apuesta
        this.gameObject.GetComponent<Bet>().ResetCanvas();
    }

    //Fin del código

}
