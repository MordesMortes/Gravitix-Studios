using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class FinalScoreScript : RealtimeComponent
{
    private FinalScoreModel _model;
    public int _finalScore;
    
    

    private FinalScoreModel model
    {
        set 
        {
            _model = value;
        }
    }
   private void UpdateFinalScore()
    {
        
        _finalScore = _model.finalScore;
    }
    private void Update()
    {
        UpdateFinalScore();
    }
}
