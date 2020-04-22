using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class FinalScoreScript : RealtimeComponent
{
    public FinalScoreModel _model;
    public int _finalScore;
    public int _blockCount;
    

    private FinalScoreModel model
    {
        set 
        {
            if (_model != null)
            {
                _model.blockCountDidChange -= BlockCountDidChange;
            }
            _model = value;
            if (_model != null)
            {
                _model.blockCountDidChange += BlockCountDidChange;
            }
        }
    }
    private void BlockCountDidChange(FinalScoreModel model, int value)
    {
        UpdateBlockCount();
    }
   public void UpdateFinalScore()
    {
        
        _finalScore = _model.finalScore;
    }
    public void ResetFinalScore()
    {
        _model.finalScore = _finalScore;
    }
    public void UpdateBlockCount()
    {
        _blockCount = _model.blockCount;
    }
    public void SetBlockCount(int value)
    {
        _model.blockCount = value;
        
    }
    
}
