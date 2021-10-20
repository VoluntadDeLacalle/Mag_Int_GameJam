using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CustomPropertyDrawer(typeof(Objective))]
public class ObjectiveEditorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect rectFoldout = new Rect(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(rectFoldout, property.isExpanded, label);
        int lines = 1;
        if (property.isExpanded)
        {
            Rect rectType = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(rectType, property.FindPropertyRelative("goalType"));
            Objective.GoalType currentType = (Objective.GoalType)property.FindPropertyRelative("goalType").enumValueIndex;

            Rect rectDescription = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rectDescription, "Objective Description");

            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textField);
            textAreaStyle.wordWrap = true;

            EditorGUI.BeginChangeCheck();
            Rect rectTextArea = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight * 3);
            string input = EditorGUI.TextArea(rectTextArea, property.FindPropertyRelative("objectiveDescription").stringValue, textAreaStyle);
            if (EditorGUI.EndChangeCheck())
            {
                property.FindPropertyRelative("objectiveDescription").stringValue = input;
            }

            lines += 2;

            switch (currentType)
            {
                case Objective.GoalType.Craft:
                    Rect rectCraftItemName = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectCraftItemName, property.FindPropertyRelative("itemName"));
                    break;
                case Objective.GoalType.Gather:
                    Rect rectGatherItemName = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectGatherItemName, property.FindPropertyRelative("itemName"));

                    Rect rectGatherItemType = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectGatherItemType, property.FindPropertyRelative("itemType"));

                    Rect rectNumbToCollect = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectNumbToCollect, property.FindPropertyRelative("numberToCollect"));

                    Rect rectItemsCollected = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectItemsCollected, property.FindPropertyRelative("collectedAmount"));
                    break;
                case Objective.GoalType.Location:
                    Rect rectRadiusToTarget = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectRadiusToTarget, property.FindPropertyRelative("activationRadius"));

                    Rect rectLocationWorldTarget = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectLocationWorldTarget, property.FindPropertyRelative("targetWorldPosition"));
                    break;
                case Objective.GoalType.Talk:
                    Rect rectNPCName = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectNPCName, property.FindPropertyRelative("npcName"));

                    Rect rectNPCText = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectNPCText, property.FindPropertyRelative("npcDialogue"));

                    Rect rectNPCSprite = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectNPCSprite, property.FindPropertyRelative("npcSprite"));
                    break;
                case Objective.GoalType.Activate:
                    Rect rectActivateItemName = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectActivateItemName, property.FindPropertyRelative("itemName"));
                    break;
                case Objective.GoalType.Restore:
                    Rect rectRestoreItemName = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
                    EditorGUI.PropertyField(rectRestoreItemName, property.FindPropertyRelative("itemName"));
                    break;
            }

            Rect rectEventSpace = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rectEventSpace, " ");

            Rect rectEvent = new Rect(position.min.x, position.min.y + lines++ * EditorGUIUtility.singleLineHeight, position.size.x, EditorGUIUtility.singleLineHeight * 4);
            EditorGUI.PropertyField(rectEvent, property.FindPropertyRelative("OnObjectiveComplete"));
        }
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int totalLines = 2;

        if (property.isExpanded)
        {
            Objective.GoalType currentType = (Objective.GoalType)property.FindPropertyRelative("goalType").enumValueIndex;

            switch (currentType)
            {
                case Objective.GoalType.Craft:
                    totalLines += 10;
                    break;
                case Objective.GoalType.Gather:
                    totalLines += 13;
                    break;
                case Objective.GoalType.Location:
                    totalLines += 11;
                    break;
                case Objective.GoalType.Talk:
                    totalLines += 12;
                    break;
                case Objective.GoalType.Activate:
                    totalLines += 10;
                    break;
                case Objective.GoalType.Restore:
                    totalLines += 10;
                    break;
            }

            if (property.FindPropertyRelative("OnObjectiveComplete").arraySize > 0)
            {
                Debug.Log(property.FindPropertyRelative("OnObjectiveComplete").arraySize);
                totalLines += (property.FindPropertyRelative("OnObjectiveComplete").arraySize) * 4;
            }   
        }

        return EditorGUIUtility.singleLineHeight * totalLines + EditorGUIUtility.standardVerticalSpacing * (totalLines - 1);
    }
}