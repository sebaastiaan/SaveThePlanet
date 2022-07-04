using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum TipoBloque
{
    Normal,
    FullVagones,
    Trenes
}

public class Bloque : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private TipoBloque tipoBloque;
    [SerializeField] private bool tieneRampa;
    
    [Header("Tren")]
    [SerializeField] private Tren[] trenes;
    
    [Header("Diamantes")]
    [SerializeField] private GameObject[] diamantes;

    [Header("Potenciadores")] 
    [SerializeField] private float probabilidadMinima;
    [SerializeField] private GameObject[] potenciadores;
    
    public TipoBloque TipoDeBloque => tipoBloque;
    public bool TieneRampa => tieneRampa;

    private List<GameObject> diamantesLista = new List<GameObject>();
    private bool diamantesReferenciados;
    private Tren trenSeleccionado;
    
    public void InicializarBloque()
    {
        if (tipoBloque == TipoBloque.Trenes)
        {
            SeleccionarTren();
        }
        
        ObtenerDiamantes();
        ActivarDiamantes();
        SeleccionarPotenciador();
    }

    private void SeleccionarPotenciador()
    {
        if (potenciadores == null || potenciadores.Length == 0)
        {
            return;
        }

        for (int i = 0; i < potenciadores.Length; i++)
        {
            potenciadores[i].SetActive(false);
        }

        float probabilidadRandom = Random.Range(0f, 100f);
        if (probabilidadRandom <= probabilidadMinima)
        {
            int itemRandomIndex = Random.Range(0, potenciadores.Length);
            potenciadores[itemRandomIndex].SetActive(true);
        }
    }
    
    private void ObtenerDiamantes()
    {
        if (diamantesReferenciados)
        {
            return;
        }

        foreach (GameObject parent in diamantes)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject diamante = parent.transform.GetChild(i).gameObject;
                diamantesLista.Add(diamante);
            }
        }

        diamantesReferenciados = true;
    }

    private void ActivarDiamantes()
    {
        if (diamantesLista.Count == 0 || diamantesLista == null)
        {
            return;
        }

        foreach (GameObject diamante in diamantesLista)
        {
            diamante.SetActive(true);
        }
    }
    
    private void SeleccionarTren()
    {
        if (trenes == null || trenes.Length == 0)
        {
            return;
        }
        
        int index = Random.Range(0, trenes.Length);
        trenes[index].gameObject.SetActive(true);
        trenSeleccionado = trenes[index];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (trenSeleccionado != null)
            {
                trenSeleccionado.PuedeMoverse = true;
                trenSeleccionado.Player = other.GetComponent<PlayerController>();
            }
        }
    }
}
