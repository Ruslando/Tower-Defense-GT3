using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    public static Action<Node> OnNodeSelected;
    public static Action OnTurretSold;
    public Turret2D Turret { get; set; }

    public void SetTurret(Turret2D turret)
    {
        Turret = turret;
        turret.SetIsEditing(true);
    }

    public void ResetTurret()
    {
        if(Turret != null) {
            Destroy(Turret.gameObject);
            Turret = null;
        }
    }

    public bool IsEmpty()
    {
        return Turret == null;
    }
    
    public void SelectTurret()
    {
        OnNodeSelected?.Invoke(this);
        if (!IsEmpty())
        {
            // ShowTurretInfo();
        }
    }

    public void SellTurret()
    {
        if (!IsEmpty())
        {
            CurrencySystem.Instance.AddCoins(Turret.GetResellValue());
            Destroy(Turret.gameObject);
            Turret = null;
            OnTurretSold?.Invoke();
        }
    }
}
