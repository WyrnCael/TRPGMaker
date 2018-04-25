﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(SpecializedClass))]
public class SpecializedClassEditor : Editor {

    private ReorderableList listTags;
    private ReorderableList listSlots;
    private ReorderableList listAttributes;
    //private ReorderableList listFormulas;
    private ReorderableList listSkills;
    private SpecializedClass specializedClass;

    List<bool> foldout;
    private Vector2 scrollPosition;

    // Get attributes SpecialClass
    List<Attribute> attributes;
    private string[] attributesStringArray;
    private int formulaAttributeSelected = -1;
    private int slotTypeSelected = -1;
    private int slotItemSelected = -1;

    private void reloadAttributes()
    {
        specializedClass = (SpecializedClass) target;
        specializedClass.refreshAttributes();
        attributes = specializedClass.attributes;

        // String array of class atributes
        attributesStringArray = new string[0];
        attributesStringArray = attributes.Select(I => I.name).ToArray();

        foldout = Enumerable.Repeat(false, attributes.Count).ToList();
    }

    private void OnEnable()
    {

        // Get tags
		listTags = new ReorderableList(serializedObject,
			serializedObject.FindProperty("tags"),
			true, true, true, true);

        // Get Slots
        listSlots = new ReorderableList(serializedObject,
                serializedObject.FindProperty("slots"),
                true, true, true, true);

        // Get Attributes
        listAttributes = new ReorderableList(serializedObject,
                serializedObject.FindProperty("attributes"),
                false, true, true, true);

        // Get Formulas
        /*listFormulas = new ReorderableList(serializedObject,
                serializedObject.FindProperty("formulas"),
                true, true, true, true);*/

        // Get Skills
        listSkills = new ReorderableList(serializedObject,
                serializedObject.FindProperty("skills"),
                true, true, true, true);

        reloadAttributes();

		// Draw Tags
		listTags.drawElementCallback =
			(Rect rect, int index, bool isActive, bool isFocused) => {
			rect.y += 2;
            EditorGUI.LabelField(
				new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
				specializedClass.tags[index]);
		};

        // Draw Slots
        listSlots.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.y += 2;

                slotTypeSelected = Database.Instance.slotTypes.IndexOf(specializedClass.slots[index].slotType);
                slotItemSelected = Database.Instance.items.IndexOf(specializedClass.slots[index].modifier);

                EditorGUI.BeginChangeCheck();
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), "Slot type:");
                slotTypeSelected =  EditorGUI.Popup(new Rect(rect.x + 63, rect.y, 100, EditorGUIUtility.singleLineHeight), slotTypeSelected, Database.Instance.slotTypes.ToArray());
                EditorGUI.LabelField(new Rect(rect.x + 170, rect.y, 35, EditorGUIUtility.singleLineHeight), "Item:");
                slotItemSelected = EditorGUI.Popup(new Rect(rect.x + 208, rect.y, rect.width - 208, EditorGUIUtility.singleLineHeight), slotItemSelected, Database.Instance.items.Select(s => (string)s.name).ToArray());
                if (EditorGUI.EndChangeCheck())
                {
                    if(slotTypeSelected != -1)
                        specializedClass.slots[index].slotType = Database.Instance.slotTypes[slotTypeSelected];
                    if(slotItemSelected != -1)
                        specializedClass.slots[index].modifier = Database.Instance.items[slotItemSelected];
                }
            };
        
        // Draw attributes
        listAttributes.drawElementCallback =
             (Rect rectL, int index, bool isActive, bool isFocused) => {
                 var element = listAttributes.serializedProperty.GetArrayElementAtIndex(index);
                 var textDimensions = GUI.skin.label.CalcSize(new GUIContent(element.displayName));
                 rectL.y += 2;

                 foldout[index] = EditorGUI.Foldout(new Rect(rectL.x, rectL.y, textDimensions.x + 5, rectL.height), foldout[index], element.displayName);
                 if (foldout[index])
                 {
                     rectL.height = EditorGUIUtility.singleLineHeight;
                     rectL.x += 15;
                     rectL.y += EditorGUIUtility.singleLineHeight;
                     EditorGUI.PropertyField(rectL, element.FindPropertyRelative("value"));
                     rectL.y += EditorGUIUtility.singleLineHeight;
                     EditorGUI.PropertyField(rectL, element.FindPropertyRelative("minValue"));
                     rectL.y += EditorGUIUtility.singleLineHeight;
                     EditorGUI.PropertyField(rectL, element.FindPropertyRelative("maxValue"));
                     listAttributes.elementHeight = EditorGUIUtility.singleLineHeight * 4.0f + 4.0f;
                 } 
                 else
                 {

                     listAttributes.elementHeight = EditorGUIUtility.singleLineHeight + 4.0f;
                 }
             };

        listAttributes.elementHeightCallback += (idx) => {
            if (foldout[idx]) return EditorGUIUtility.singleLineHeight * 4.0f +4.0f;
            else return EditorGUIUtility.singleLineHeight + 4.0f;
        };

        // Draw formulas
        /*listFormulas.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = listFormulas.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.BeginChangeCheck();
                formulaAttributeSelected = attributes.IndexOf(specializedClass.formulas[index].attributeModified);
                if (formulaAttributeSelected < 0) formulaAttributeSelected = 0;
                formulaAttributeSelected = EditorGUI.Popup(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    formulaAttributeSelected, attributesStringArray);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 70, rect.y, 60, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("operation"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + 140, rect.y, 90, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("value"), GUIContent.none);
                if (formulaAttributeSelected < 0)
                    specializedClass.formulas.Remove(specializedClass.formulas[index]);
                if (EditorGUI.EndChangeCheck())
                {
                    specializedClass.formulas[index].setAttributeModified(attributes[formulaAttributeSelected]);
                }
            };*/

        // Draw Skills
        listSkills.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) => {
                rect.y += 2;
                if(specializedClass.skills[index] != null)
                    EditorGUI.LabelField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        specializedClass.skills[index].name);
            };

        // Tags header
        listTags.drawHeaderCallback = (Rect rect) => {
			EditorGUI.LabelField(rect, "Tags");
		};

        // Slots header
        listSlots.drawHeaderCallback = (Rect rect) => {
        	EditorGUI.LabelField(rect, "Slots");
       	};

        // listFormulas header
        /*listFormulas.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Formulas");
        };*/

        // listSkills header
        listSkills.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Skills");
        };

        // listAttributes header
        listAttributes.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "Attributes");
        };

		// Add tags
		listTags.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
			var menu = new GenericMenu();
			foreach (string tag in Database.Instance.tags)
			{
				menu.AddItem(new GUIContent(tag),
						false, clickHandlerTags,
					tag);
				
			}
			menu.ShowAsContext();
		};


        // Add slot
        listSlots.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            specializedClass.slots.Add(new Slot());
        };

        // Add attribute
        listAttributes.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            foreach (Attribute attrib in Database.Instance.attributes)
            {
                if (!attrib.isCore && !attributes.Contains(attrib)) {
                    menu.AddItem(new GUIContent(attrib.name),
						false, clickHandlerAttributes,
                    attrib);
                }
            }
            menu.ShowAsContext();
        };

        // Remove attribute
        listAttributes.onCanRemoveCallback = (ReorderableList l) => {
            if (attributes[l.index].isCore)
                return false;
            else
                return true;
        };

        // Add skill
        listSkills.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            foreach (Skills skill in Database.Instance.skills)
            {
                if (!specializedClass.skills.Contains(skill))
                {
                    menu.AddItem(new GUIContent(skill.name),
                        false, clickHandlerSkills,
                    skill);
                }
            }
            menu.ShowAsContext();
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Clean array if there are null objects
        for (int i = 0; i < listSkills.serializedProperty.arraySize; i++)
        {
            var elementProperty = listSkills.serializedProperty.GetArrayElementAtIndex(i);
            if (elementProperty.objectReferenceValue == null)
                listSkills.serializedProperty.DeleteArrayElementAtIndex(i);
        }

        var customStyle = new GUIStyle();
        customStyle.alignment = TextAnchor.UpperCenter;
        customStyle.fontSize = 17;
        GUI.Label(new Rect(EditorGUILayout.GetControlRect().x, EditorGUILayout.GetControlRect().y, EditorGUILayout.GetControlRect().width, 30), "Editing \"" + specializedClass.name +  "\" specialized class:", customStyle);

        EditorGUILayout.BeginVertical();
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("name"), new GUIContent("Name: "), GUILayout.MinWidth(100));
        if (EditorGUI.EndChangeCheck())
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath((SpecializedClass)target), specializedClass.name + ".asset");
        }

        listAttributes.DoLayoutList();
        listTags.DoLayoutList();
		listSlots.DoLayoutList();        
        listSkills.DoLayoutList();
        //listFormulas.DoLayoutList();

        // For formulas  
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("Formula:", EditorStyles.boldLabel);
        var f = specializedClass.formula;
        f.formula = EditorGUILayout.TextField(f.formula);

        if (!f.FormulaParser.IsValidExpression)
        {
            EditorGUILayout.LabelField(f.FormulaParser.Error);
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(specializedClass);
    }

	private void clickHandlerAttributes(object target)
    {
        var data = (Attribute) target;
        attributes.Add(data);
        foldout.Add(false);
        serializedObject.ApplyModifiedProperties();
    }

	private void clickHandlerTags(object target)
	{
		var data = (string) target;
		specializedClass.tags.Add (data);
		serializedObject.ApplyModifiedProperties();
	}

    private void clickHandlerSkills(object target)
    {
        var data = (Skills)target;
        specializedClass.skills.Add(data);
        serializedObject.ApplyModifiedProperties();
    }    
}
