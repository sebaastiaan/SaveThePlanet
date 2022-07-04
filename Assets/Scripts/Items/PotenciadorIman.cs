using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotenciadorIman : MonoBehaviour
{
    public static event Action<float> EventoIman;
    [SerializeField] private float duracion;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instancia.ReproducirSonidoFX(SoundManager.Instancia.itemClip);
            EventoIman?.Invoke(duracion);
            gameObject.SetActive(false);
        }
    }
}
