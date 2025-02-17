using System;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;

using ImGuizmoNET;

using Dalamud;
using Dalamud.Logging;
using Dalamud.Configuration;
using Dalamud.Game.ClientState.Keys;

using Ktisis.Localization;
using Ktisis.Structs.Actor.Equip.SetSources;
using Ktisis.Interface;
using Ktisis.Structs.Bones;

namespace Ktisis {
	[Serializable]
	public class Configuration : IPluginConfiguration {
		public const int CurVersion = 0;
		public int Version { get; set; } = CurVersion;

		// Interface

		public bool AutoOpen { get; set; } = true;
		public bool AutoOpenCtor { get; set; } = false;

		public bool DisplayCharName { get; set; } = true;
		public bool CensorNsfw { get; set; } = true;

		public bool TransformTableDisplayMultiplierInputs { get; set; } = false;
		public float TransformTableBaseSpeedPos { get; set; } = 0.0005f;
		public float TransformTableBaseSpeedRot { get; set; } = 0.1f;
		public float TransformTableBaseSpeedSca { get; set; } = 0.001f;
		public float TransformTableModifierMultCtrl { get; set; } = 0.1f;
		public float TransformTableModifierMultShift { get; set; } = 10f;
		public int TransformTableDigitPrecision { get; set; } = 3;

		// Input
		public bool EnableKeybinds { get; set; } = true;
		public Dictionary<Input.Purpose, List<VirtualKey>> KeyBinds { get; set; } = new();

		public bool DisableChangeTargetOnLeftClick { get; set; } = false;
		public bool DisableChangeTargetOnRightClick { get; set; } = false;

		// Overlay

		public bool DrawLinesOnSkeleton { get; set; } = true;
		public float SkeletonLineThickness { get; set; } = 2.0F;
		public float SkeletonDotRadius { get; set; } = 3.0F;

		// References
		// The reference Key creates a uniqueness constraint for imgui window IDs for each reference.
		public Dictionary<int, ReferenceInfo> References { get; set; } = new();
		public float ReferenceAlpha { get; set; } = 1.0f;
		public bool ReferenceHideDecoration { get; set; } = false;
		public int NextReferenceKey => References.Count == 0 ? 0 : References.Max(x => x.Key) + 1;

		public Vector4 GetCategoryColor(Bone bone) {
			if (LinkBoneCategoryColors) return LinkedBoneCategoryColor;
			// pick the first category found
			foreach (var category in bone.Categories)
				if (IsBoneCategoryVisible(category) && BoneCategoryColors.TryGetValue(category.Name, out Vector4 color))
					return color;
			return LinkedBoneCategoryColor;
		}

		public bool IsBoneVisible(Bone bone) {
			// Check if input is forcing a category to show solo
			if (Category.VisibilityOverload.Count > 0)
				if (Category.VisibilityOverload.Intersect(bone.Categories.Select(c => c.Name)).Any())
					return true;
				else
					return false;

			if (CensorNsfw && bone.Categories.Any(c => c.IsNsfw))
				return false;

			// bone will be visible if any category is visible
			foreach (var category in bone.Categories)
				if (ShowBoneByCategory.GetValueOrDefault(category.Name, true))
					return true;

			return false;
		}

		public bool IsBoneCategoryVisible(Category category) {
			if (!ShowBoneByCategory.TryGetValue(category.Name, out bool boneCategoryVisible))
				return true;
			return boneCategoryVisible;
		}

		// Gizmo

		public MODE GizmoMode { get; set; } = MODE.LOCAL;
		public OPERATION GizmoOp { get; set; } = OPERATION.TRANSLATE;

		public Overlay.Skeleton.SiblingLink SiblingLink { get; set; } = Overlay.Skeleton.SiblingLink.None;

		public bool AllowAxisFlip { get; set; } = true;

		// Language

		public UserLocale Localization { get; set; } = UserLocale.English;
		public ClientLanguage SheetLocale { get; set; } = ClientLanguage.English;

		public bool TranslateBones = true;

		// UI memory

		public bool ShowSkeleton { get; set; } = false;
		public Dictionary<string, bool> ShowBoneByCategory = new();
		public bool LinkBoneCategoryColors { get; set; } = false;
		public Vector4 LinkedBoneCategoryColor { get; set; } = new(1.0F, 1.0F, 1.0F, 0.5647059F);
		public Dictionary<string, Vector4> BoneCategoryColors = new();

		public bool EnableParenting { get; set; } = true;

		public bool LinkedGaze { get; set; } = true;

		// Data memory
		public Dictionary<string, GlamourDresser.GlamourPlate[]?>? GlamourPlateData { get; set; } = null;
		public Dictionary<string, Dictionary<string, Vector3>> CustomBoneOffset { get; set; } = new();

		// Validate for changes in config versions.

		public void Validate() {
			if (Version == CurVersion)
				return;

			PluginLog.Warning($"Updating config to reflect changes between config versions {Version}-{CurVersion}.\nThis is nothing to worry about, but some settings may change or get reset!");

			//switch (Version) {}

			Version = CurVersion;
		}
	}
	[Serializable]
	public class ReferenceInfo {
		public bool Showing { get; set; }
		public string? Path { get; set; }
	}
}