using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item)), CanEditMultipleObjects]
public class ItemEditor : Editor
{
    public SerializedProperty
        type_Prop,
        name_Prop,
        description_Prop,
        equip_Prop,
        image_Prop,
        componentTransforms_Prop;

    private void OnEnable()
    {
        type_Prop = serializedObject.FindProperty("itemType");
        name_Prop = serializedObject.FindProperty("itemName");
        description_Prop = serializedObject.FindProperty("description");
        equip_Prop = serializedObject.FindProperty("isEquipped");
        image_Prop = serializedObject.FindProperty("inventorySprite");
        componentTransforms_Prop = serializedObject.FindProperty("chassisComponentTransforms");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(type_Prop);
        Item.TypeTag currentType = (Item.TypeTag)type_Prop.enumValueIndex;

        switch (currentType)
        {
            case Item.TypeTag.chassis:
                EditorGUILayout.PropertyField(name_Prop, new GUIContent("Item Name"));
                EditorGUILayout.PropertyField(description_Prop, new GUIContent("Description"));
                EditorGUILayout.PropertyField(equip_Prop, new GUIContent("Is Equipped"));
                EditorGUILayout.PropertyField(image_Prop, new GUIContent("Inventory Sprite"));
                EditorGUILayout.PropertyField(componentTransforms_Prop, new GUIContent("Chassis Component Transforms"));
                break;
            case Item.TypeTag.activeComponent:
                EditorGUILayout.PropertyField(name_Prop, new GUIContent("Item Name"));
                EditorGUILayout.PropertyField(description_Prop, new GUIContent("Description"));
                EditorGUILayout.PropertyField(image_Prop, new GUIContent("Inventory Sprite"));
                break;
            case Item.TypeTag.grip:
                EditorGUILayout.PropertyField(name_Prop, new GUIContent("Item Name"));
                EditorGUILayout.PropertyField(description_Prop, new GUIContent("Description"));
                EditorGUILayout.PropertyField(image_Prop, new GUIContent("Inventory Sprite"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
