using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Item), true)][CanEditMultipleObjects]
public class ItemEditor : Editor
{
    public SerializedProperty
        type_Prop,
        name_Prop,
        description_Prop,
        equip_Prop,
        localHandPos_Prop,
        localHandRot_Prop,
        image_Prop,
        componentTransforms_Prop,
        gripTransform_Prop;

    private void OnEnable()
    {
        type_Prop = serializedObject.FindProperty("itemType");
        name_Prop = serializedObject.FindProperty("itemName");
        description_Prop = serializedObject.FindProperty("description");
        equip_Prop = serializedObject.FindProperty("isEquipped");
        localHandPos_Prop = serializedObject.FindProperty("localHandPos");
        localHandRot_Prop = serializedObject.FindProperty("localHandRot");
        image_Prop = serializedObject.FindProperty("inventorySprite");
        componentTransforms_Prop = serializedObject.FindProperty("chassisComponentTransforms");
        gripTransform_Prop = serializedObject.FindProperty("chassisGripTransform");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        FieldInfo[] childFields = target.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        EditorGUILayout.PropertyField(type_Prop);
        Item.TypeTag currentType = (Item.TypeTag)type_Prop.enumValueIndex;

        switch (currentType)
        {
            case Item.TypeTag.chassis:
                EditorGUILayout.PropertyField(name_Prop, new GUIContent("Item Name"));
                EditorGUILayout.PropertyField(description_Prop, new GUIContent("Description"));
                EditorGUILayout.PropertyField(equip_Prop, new GUIContent("Is Equipped"));
                EditorGUILayout.PropertyField(localHandPos_Prop, new GUIContent("Local Hand Position"));
                EditorGUILayout.PropertyField(localHandRot_Prop, new GUIContent("Local Hand Rotation"));
                EditorGUILayout.PropertyField(image_Prop, new GUIContent("Inventory Sprite"));
                EditorGUILayout.PropertyField(componentTransforms_Prop, new GUIContent("Chassis Component Transforms"));
                EditorGUILayout.PropertyField(gripTransform_Prop, new GUIContent("Chassis Grip Transform"));
                break;
            case Item.TypeTag.effector:
                EditorGUILayout.PropertyField(name_Prop, new GUIContent("Item Name"));
                EditorGUILayout.PropertyField(description_Prop, new GUIContent("Description"));
                EditorGUILayout.PropertyField(equip_Prop, new GUIContent("Is Equipped"));
                EditorGUILayout.PropertyField(image_Prop, new GUIContent("Inventory Sprite"));
                break;
            case Item.TypeTag.grip:
                EditorGUILayout.PropertyField(name_Prop, new GUIContent("Item Name"));
                EditorGUILayout.PropertyField(description_Prop, new GUIContent("Description"));
                EditorGUILayout.PropertyField(equip_Prop, new GUIContent("Is Equipped"));
                EditorGUILayout.PropertyField(image_Prop, new GUIContent("Inventory Sprite"));
                break;
        }

        if (target.GetType().Name != "Item")
        {
            foreach (FieldInfo field in childFields)
            {

                if (field.IsPublic || field.GetCustomAttribute(typeof(SerializeField)) != null)
                {

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
