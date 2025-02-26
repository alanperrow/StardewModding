﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Locations;
using StardewValley.Objects;

namespace ConvenientInventory.TypedChests
{
    /// <summary>
    /// Wrapper for <see cref="StardewValley.Objects.Chest"/>, holding information about what type of chest this is.
    /// </summary>
    public class TypedChest : ITypedChest
    {
        public TypedChest(Chest chest, ChestType chestType, GameLocation chestGameLocation, Vector2? visualTileLocation)
        {
            this.Chest = chest;
            this.ChestType = chestType;
            this.ChestGameLocation = chestGameLocation;
            this.VisualTileLocation = visualTileLocation;
        }

        public Chest Chest { get; }

        public ChestType ChestType { get; }

        public GameLocation ChestGameLocation { get; }

        public Vector2? VisualTileLocation { get; }

        public static ChestType DetermineChestType(Chest chest)
        {
            if (chest.SpecialChestType == Chest.SpecialChestTypes.BigChest)
            {
                return chest.QualifiedItemId == "(BC)BigStoneChest" ? ChestType.BigStone : ChestType.BigNormal;
            }

            if (chest.SpecialChestType != Chest.SpecialChestTypes.None)
            {
                return ChestType.Special;
            }

            if (chest.Location is VolcanoDungeon)
            {
                // Chests cannot be manually placed in a dungeon, so this must be a generated dungeon chest.
                return ChestType.Dungeon;
            }

            return chest.ParentSheetIndex switch
            {
                232 => ChestType.Stone,
                216 => ChestType.MiniFridge,
                -1 => ChestType.Package,
                _ => ChestType.Normal,
            };
        }

        public bool IsBuildingChestType()
        {
            return this.ChestType == ChestType.Mill || this.ChestType == ChestType.JunimoHut;
        }

        public int DrawInToolTip(SpriteBatch spriteBatch, Point toolTipPosition, int posIndex)
        {
            int x = toolTipPosition.X + 20 + 46 * (posIndex % 8);
            int y = toolTipPosition.Y + 40 + 52 * (posIndex / 8);

            return this.ChestType switch
            {
                ChestType.Normal => DrawNormalChestTooltip(spriteBatch, x, y),
                ChestType.BigNormal => DrawBigNormalChestTooltip(spriteBatch, x, y),
                ChestType.Stone or ChestType.BigStone => DrawStoneChestTooltip(spriteBatch, x, y),
                ChestType.MiniFridge => DrawMiniFridgeTooltip(spriteBatch, x, y),
                ChestType.Fridge => DrawFridgeTooltip(spriteBatch, x, y),
                ChestType.IslandFridge => DrawIslandFridgeTooltip(spriteBatch, x, y),
                ChestType.Mill => DrawMillTooltip(spriteBatch, toolTipPosition, posIndex, x, y),
                ChestType.JunimoHut => DrawJunimoHutTooltip(spriteBatch, toolTipPosition, posIndex, x, y),
                ChestType.Dresser => DrawDresserTooltip(spriteBatch, x, y),
                _ => DrawSpecialChestTooltip(spriteBatch, x, y),
            };
        }

        private int DrawNormalChestTooltip(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                new Vector2(x, y),
                Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, !this.Chest.playerChoiceColor.Value.Equals(Color.Black) ? 168 : this.Chest.ParentSheetIndex, 16, 32),
                this.Chest.playerChoiceColor.Value.Equals(Color.Black) ? this.Chest.Tint : this.Chest.playerChoiceColor.Value,
                0f, Vector2.Zero, 2f, SpriteEffects.None, 1f
            );

            spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                new Vector2(x, y + 42),
                new Rectangle(0, 168 / 8 * 32 + 53, 16, 11),
                Color.White,
                0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 1E-05f
            );

            spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                new Vector2(x, y),
                Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.startingLidFrame.Value + 46, 16, 32),
                Color.White,
                0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 2E-05f
            );

            return 0;
        }

        private int DrawBigNormalChestTooltip(SpriteBatch spriteBatch, int x, int y)
        {
            ParsedItemData chestItemData = ItemRegistry.GetDataOrErrorItem(this.Chest.QualifiedItemId);
            Texture2D chestTexture = chestItemData.GetTexture();
            Color chestColor = this.Chest.playerChoiceColor.Value;

            if (chestColor.Equals(Color.Black))
            {
                spriteBatch.Draw(chestTexture, new Vector2(x, y), chestItemData.GetSourceRect(), this.Chest.Tint, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
                spriteBatch.Draw(chestTexture, new Vector2(x, y), chestItemData.GetSourceRect(0, this.Chest.startingLidFrame.Value), this.Chest.Tint, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f + 1E-05f);
            }
            else
            {
                int spriteIndex = 312;
                int lidIndex = this.Chest.startingLidFrame.Value + 16;
                int coloredLidIndex = this.Chest.startingLidFrame.Value + 8;
                Rectangle drawRect = chestItemData.GetSourceRect(0, spriteIndex);
                Rectangle lidRect = chestItemData.GetSourceRect(0, lidIndex);
                Rectangle coloredLidRect = chestItemData.GetSourceRect(0, coloredLidIndex);

                spriteBatch.Draw(chestTexture, new Vector2(x, y), drawRect, chestColor, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f);
                spriteBatch.Draw(chestTexture, new Vector2(x, y + 20), new Rectangle(0, spriteIndex / 8 * 32 + 53, 16, 11), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f + 2E-05f);
                spriteBatch.Draw(chestTexture, new Vector2(x, y), coloredLidRect, chestColor, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f + 1E-05f);
                spriteBatch.Draw(chestTexture, new Vector2(x, y), lidRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 1f + 2E-05f);
            }

            return 0;
        }
        private int DrawStoneChestTooltip(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
            new Vector2(x, y),
                Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.ParentSheetIndex, 16, 32),
                this.Chest.playerChoiceColor.Value.Equals(Color.Black) ? this.Chest.Tint : this.Chest.playerChoiceColor.Value,
                0f, Vector2.Zero, 2f, SpriteEffects.None, 1f
            );

            spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                new Vector2(x, y + 42),
                new Rectangle(0, this.Chest.ParentSheetIndex / 8 * 32 + 53, 16, 11),
                Color.White,
                0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 1E-05f
            );

            spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                new Vector2(x, y),
                Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.startingLidFrame.Value + 8, 16, 32),
                Color.White,
                0f, Vector2.Zero, 2f, SpriteEffects.None, 1f - 2E-05f
            );

            return 0;
        }

        private int DrawMiniFridgeTooltip(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                new Vector2(x, y + 8),
                Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.ParentSheetIndex, 16, 32),
                Color.White,
                0f, Vector2.Zero, 1.75f, SpriteEffects.None, 1f
            );

            return 0;
        }

        private static int DrawFridgeTooltip(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(CachedTextures.FarmHouse,
                new Vector2(x, y + 3),
                new Rectangle(16 * 5, 48 * 4 + 13, 16, 35),
                Color.White,
                0f, Vector2.Zero, 1.75f, SpriteEffects.None, 1f);

            return 0;
        }

        private static int DrawIslandFridgeTooltip(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(CachedTextures.FarmHouse,
                new Vector2(x, y + 3),
                new Rectangle(16 * 6, 48 * 6 + 29, 16, 35),
                Color.White,
                0f, Vector2.Zero, 1.75f, SpriteEffects.None, 1f);

            return 0;
        }

        private static int DrawMillTooltip(SpriteBatch spriteBatch, Point toolTipPosition, int posIndex, int x, int y)
        {
            int newPosIndex = posIndex;

            if (posIndex % 8 == 7)
            {
                newPosIndex++;
                x = toolTipPosition.X + 20 + 46 * (newPosIndex % 8);
                y = toolTipPosition.Y + 40 + 52 * (newPosIndex / 8);
            }

            spriteBatch.Draw(CachedTextures.Mill,
                new Vector2(x + 8, y),
                new Rectangle(0, 64, 64, 64),
                Color.White,
                0f, Vector2.Zero, 1f, SpriteEffects.None, 1f
            );

            newPosIndex++;

            return newPosIndex - posIndex;
        }

        private static int DrawJunimoHutTooltip(SpriteBatch spriteBatch, Point toolTipPosition, int posIndex, int x, int y)
        {
            int newPosIndex = posIndex;

            if (posIndex % 8 == 7)
            {
                newPosIndex++;
                x = toolTipPosition.X + 20 + 46 * (newPosIndex % 8);
                y = toolTipPosition.Y + 40 + 52 * (newPosIndex / 8);
            }

            spriteBatch.Draw(CachedTextures.JunimoHut,
                new Vector2(x + 9, y - 16),
                new Rectangle(Utility.getSeasonNumber(Game1.currentSeason) * 48, 0, 48, 64),
                Color.White,
                0f, Vector2.Zero, 1.25f, SpriteEffects.None, 1f
            );

            newPosIndex++;

            return newPosIndex - posIndex;
        }

        private int DrawDresserTooltip(SpriteBatch spriteBatch, int x, int y)
        {
            if (this.Chest is not DresserFakeChest dresserFakeChest)
            {
                return 0;
            }

            ParsedItemData dresserItemData = ItemRegistry.GetDataOrErrorItem(dresserFakeChest.Dresser.QualifiedItemId);
            Texture2D dresserTexture = dresserItemData.GetTexture();
            Rectangle dresserSourceRect = dresserItemData.GetSourceRect();

            spriteBatch.Draw(dresserTexture,
                new Vector2(x - 8, y + 12),
                dresserSourceRect,
                Color.White,
                0f, Vector2.Zero, 1.5f, SpriteEffects.None, 1f
            );

            return 0;
        }

        private int DrawSpecialChestTooltip(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet,
                new Vector2(x, y),
                Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, this.Chest.ParentSheetIndex, 16, 32),
                Color.White,
                0f, Vector2.Zero, 2f, SpriteEffects.None, 1f
            );

            return 0;
        }
    }
}
