using System;
using System.Collections.Generic;
using UnityEngine;

public class GameData {
    public int Round;
    public int TotalScore;
    public int TimeLeft;
    
    public RecipeSO.RecipeData NewRecipe;
    public List<int> RecipeDelivered = null;
    public int RecipeTimeout; // Timeout is not implemented yet, possibly never will be
    public List<RecipeSO.RecipeData> RecipeList = null;
    
    public Vector2 PlayerPosition;
    public List<string> PlayerHoldItems;
    
    public int FryingTimer;
    public StoveCounter.FryingState FryingState;
}