using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControllerFade : MonoBehaviour
{
    [SerializeField] static ControllerFade _instaciaFade;
    [SerializeField] Image _imageFade;
    [SerializeField] Color _corInicial;
    [SerializeField] Color _corFinal;
    [SerializeField] float _duracaoFade;
    [SerializeField] bool _isFade;
    private float _tempo;

    void Awake()
    {
        _instaciaFade = this;        
    }

    IEnumerator InicioFade()
    {
        _isFade = true;
        _tempo = 0f;

        while(_tempo <= _duracaoFade)
        {
            _imageFade.color = Color.Lerp(_corInicial, _corFinal, _tempo / _duracaoFade);
            _tempo = _tempo + Time.deltaTime;
            yield return null;
        }

        _isFade = false;

    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InicioFade());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
