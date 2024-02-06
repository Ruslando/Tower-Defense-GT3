using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    public static Action<Node> OnNodeSelected;
    public static Action OnTurretSold;

    // [SerializeField] private GameObject attackRangeSprite;
    
    // public Turret Turret { get; set; }
    public Turret2D Turret { get; set; }

    // private float _rangeSize;
    // private Vector3 _rangeOriginalSize;

    private void Start()
    {
        // _rangeSize = attackRangeSprite.GetComponent<SpriteRenderer>().bounds.size.y;
        // _rangeOriginalSize = attackRangeSprite.transform.localScale;
    }

    // public void SetTurret(Turret turret)
    // {
    //     Turret = turret;
    // }

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

    // public void CloseAttackRangeSprite()
    // {
    //     attackRangeSprite.SetActive(false);
    // }
    
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
            // attackRangeSprite.SetActive(false);
            OnTurretSold?.Invoke();
        }
    }

    // private void ShowTurretInfo()
    // {
    //     attackRangeSprite.SetActive(true);
    //     attackRangeSprite.transform.localScale = _rangeOriginalSize * Turret.AttackRange / (_rangeSize / 2);
    // }
}
