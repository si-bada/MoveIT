using UnityEngine;

public class PostController : MonoBehaviour
{
    [SerializeField] Transform m_depot;
    [Header("Status")]
    [SerializeField] int m_stock;

    public Transform depot => m_depot;


    // takes _amount elements from the stock
    public bool Pick(int _amount)
    {
        if(m_stock >= _amount)
        {
            m_stock -= _amount;
            return true;
        }
        else // not enough stock
        {
            // Handing the generation of products is out of the scope of this test
            SceneController.sIntance.ResetScene();
            return false;
        }
    }

    // adds _amount elements to the stock
    public void Drop(int _amount)
    {
        m_stock += _amount;
    }
}
