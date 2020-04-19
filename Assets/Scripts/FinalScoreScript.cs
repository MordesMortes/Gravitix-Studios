using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class FinalScoreScript : RealtimeComponent
{
    private FinalScoreModel _model;
    private int _finalScore;
   

   private FinalScoreModel model
    {
        set 
        {
            _model = value;
        }
    }
   
}
