using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsPlayerController : MonoBehaviour
{
    /// <summary>移動速度</summary>
    [SerializeField] float m_movingSpeed = 5f;
    /// <summary>走る速度</summary>
    [SerializeField] float m_runSpeed = 8f;
    /// <summary>ジャンプ力</summary>
    [SerializeField] float m_jumpPower = 3f;
    /// <summary>接地判定の際の接地している判定する長さ</summary>
    [SerializeField] float m_isGroundedLength = 1.2f;
    /// <summary> しゃがみの高さ </summary>
    [SerializeField] float m_crouchHeight = 1.2f;
    Rigidbody m_rb;


    // Start is called before the first frame update
    void Start()
    {
        m_rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 方向の入力を取得し、方向を求める
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        // 入力方向のベクトルを組み立てる
        Vector3 dir = Vector3.forward * v + Vector3.right * h;


        if (dir == Vector3.zero)
        {
            // 方向の入力がニュートラルの時は、y 軸方向の速度を保持するだけ
            m_rb.velocity = new Vector3(0f, m_rb.velocity.y, 0f);
        }
        else
        {

            // カメラを基準に入力が上下=奥/手前, 左右=左右にキャラクターを向ける
            dir = Camera.main.transform.TransformDirection(dir);    // メインカメラを基準に入力方向のベクトルを変換する
            dir.y = 0;  // y 軸方向はゼロにして水平方向のベクトルにする

            //// 入力方向に滑らかに回転させる
            //transform.rotation = Quaternion.LookRotation(dir);

            Vector3 velo = dir.normalized * m_movingSpeed; // 入力した方向に移動する
            velo.y = m_rb.velocity.y;   // ジャンプした時の y 軸方向の速度を保持する
            m_rb.velocity = velo;   // 計算した速度ベクトルをセットする

            if (Input.GetButton("Dush"))
            {
                Vector3 runVelo = dir.normalized * m_movingSpeed * 1.5f;
                runVelo.y = m_rb.velocity.y;   // ジャンプした時の y 軸方向の速度を保持する
                m_rb.velocity = runVelo;
                // 計算した速度ベクトルをセットする

            }
            
            if (IsGrounded())
            {
                // ジャンプの入力を取得し、接地している時に押されていたらジャンプする
                if (Input.GetButtonDown("Jump"))
                {
                    m_rb.AddForce(Vector3.up * m_jumpPower, ForceMode.Impulse);
                }
            }
        }

        /// <summary>
        /// 地面に接触しているか判定する
        /// </summary>
        /// <returns></returns>
        bool IsGrounded()
        {
            // Physics.Linecast() を使って足元から線を張り、そこに何かが衝突していたら true とする
            Vector3 start = this.transform.position;   // start: オブジェクトの中心
            Vector3 end = start + Vector3.down * m_isGroundedLength;  // end: start から真下の地点
            Debug.DrawLine(start, end); // 動作確認用に Scene ウィンドウ上で線を表示する
            bool isGrounded = Physics.Linecast(start, end); // 引いたラインに何かがぶつかっていたら true とする
            return isGrounded;
        }
    }
}
