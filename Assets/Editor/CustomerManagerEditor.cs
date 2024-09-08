using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomerManager))]
public class CustomerManagerEditor : Editor
{
    SerializedProperty hasEventProperty;
    SerializedProperty eventOrdersProperty;
    SerializedProperty priceProperty;
    SerializedProperty ordersDataProperty;
    SerializedProperty appearanceProbabilityProperty;

    private void OnEnable()
    {
        // Obtener referencias a las propiedades serializadas
        hasEventProperty = serializedObject.FindProperty("hasEvent");
        eventOrdersProperty = serializedObject.FindProperty("eventOrders");
        priceProperty = serializedObject.FindProperty("price");
        ordersDataProperty = serializedObject.FindProperty("ordersData");
        appearanceProbabilityProperty = serializedObject.FindProperty("appearanceProbability");
    }

    public override void OnInspectorGUI()
    {
        // Actualizar el estado del objeto serializado
        serializedObject.Update();

        // Dibujar las demás propiedades manualmente, excepto hasEvent
        EditorGUILayout.PropertyField(priceProperty);
        EditorGUILayout.PropertyField(ordersDataProperty, true);
        EditorGUILayout.PropertyField(appearanceProbabilityProperty);

        // Dibujar el valor del hasEvent en el Inspector
        EditorGUILayout.PropertyField(hasEventProperty);

        // Mostrar la lista de eventos solo si hasEvent es true
        if (hasEventProperty.boolValue)
        {
            EditorGUILayout.PropertyField(eventOrdersProperty, true);
        }

        // Aplicar cualquier cambio hecho en el Inspector
        serializedObject.ApplyModifiedProperties();
    }
}
