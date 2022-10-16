using System.Collections.Generic;
using UnityEngine;
//namespace Ozaru
public class MwCodingStdTemplate :MonoBehaviour  {


    System.Action<int> myAction;

    //Public Projected Var //camelcase
    public int testInt;
    protected int ttestInt;

    //Private Var //_ underscore
    private int _testInt;


    private void OnEnable() {
        myAction += SomeFunction;
        myAction += SomeFunction2;

        myAction = SomeFunction;
    }

    private void OnDisable() {
        myAction -= SomeFunction;
    }

    Rigidbody _rb = null;


    private void Start() {


        if (_rb) {
            //_rb.AddForce();
        }

        //_rb?.AddForce();


        myAction?.Invoke(100);
    }

    //Function UpperCase CamelCase
    void SomeFunction(int value) {
        WxLogger.Log(value);
    }

    void SomeFunction2(int value) {
        WxLogger.Log(value);
    }

    //C# Tricks
    //Getter
    private int _fieldValue = 0;
    public int fieldValue => fieldValue;
    

    //Field && Properties, Cannot be serialize
    public string stringValue { get; private set; } = "Hello World";
    public string stringValue2 { get; protected set; } = "hello world";



    //using System.Collections.Generic;
    List<int> _someList = new List<int>();
    public IReadOnlyList<int> someList => _someList;

    //Unity Att

    //[SerializeField]
    //[Min(0)]
    //[Space]
    //[Header("SomeHead")]
    //[Tooltip("213123")]


}
