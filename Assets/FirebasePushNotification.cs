using Firebase.Messaging;
using UnityEngine;

public class FirebasePushNotification : MonoBehaviour
{
    void Start()
    {
        FirebaseMessaging.TokenReceived += (a, b) => Debug.Log("Token received");
        FirebaseMessaging.MessageReceived += (a, b) => Debug.Log("Message Received");
        Subscribe();
    }

    private void Subscribe()
    {
        FirebaseMessaging.SubscribeAsync("/topics/FusionPrix");
    }
}
