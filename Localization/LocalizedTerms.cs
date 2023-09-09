namespace AF
{
    public static class LocalizedTerms
    {
        public enum LocalizedAction {
            NONE,
            PICKUP_ITEM,
            TALK,
        }

        #region Consumable Descriptions
        // TODO: Move this to a specific class manager that contains all other data (bar color, sprites) so we dont save it on the save file
        public static string GetConsumableDisplayNameEffect(Consumable.ConsumablePropertyName consumablePropertyName)
        {
            if (consumablePropertyName == Consumable.ConsumablePropertyName.HEALTH_REGENERATION)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Regenera��o de vida";
                }

                return "Health regeneration over time";
            }

            if (consumablePropertyName == Consumable.ConsumablePropertyName.JUMP_HEIGHT)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Aumento de altura de salto";
                }

                return "Increased jump height";
            }

            if (consumablePropertyName == Consumable.ConsumablePropertyName.PHYSICAL_ATTACK_BONUS)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Aumento do poder de ataque";
                }

                return "Increased physical attack";
            }

            if (consumablePropertyName == Consumable.ConsumablePropertyName.STAMINA_REGENERATION_RATE)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Regenera��o de stamina mais r�pida";
                }

                return "Faster stamina regeneration";
            }

            if (consumablePropertyName == Consumable.ConsumablePropertyName.SLOWER_STAMINA_REGENERATION_RATE)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Regenera��o de stamina mais lenta";
                }

                return "Slower stamina regeneration";
            }

            if (consumablePropertyName == Consumable.ConsumablePropertyName.ALL_STATS_INCREASE)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Aumento de Atributos";
                }

                return "Increased Stats";
            }

            if (consumablePropertyName == Consumable.ConsumablePropertyName.DEXTERITY_INCREASE)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Aumento de Destreza";
                }

                return "Increased Dexterity";
            }

            if (consumablePropertyName == Consumable.ConsumablePropertyName.STRENGTH_INCREASE)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Aumento de For�a";
                }

                return "Increased Strength";
            }

            if (consumablePropertyName == Consumable.ConsumablePropertyName.NO_DAMAGE_FOR_X_SECONDS)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Invenc�vel";
                }

                return "Invencibility";
            }

            if (consumablePropertyName == Consumable.ConsumablePropertyName.FART_ON_HIT)
            {
                if (GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.PORTUGUESE)
                {
                    return "Flatul�ncia";
                }

                return "Flatulence";
            }

            return "";
        }

        #endregion

        public static string ThisPersonIsBusy()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Esta pessoa est� ocupada.",
                _ => "This person is busy.",
            };
        }

        public static string CaughtStealing()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Foste apanhado a roubar!",
                _ => "You were caught stealing!",
            };
        }

        #region Companions

        public static string HasJoinedTheParty()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "juntou-se � equipa!",
                _ => "has joined the party!",
            };
        }
        public static string HasLeftTheParty()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "abandonou a equipa!",
                _ => "has left the party!",
            };
        }
        public static string WaitHere()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Espera aqui",
                _ => "Wait here",
            };
        }
        public static string FollowMe()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Segue-me",
                _ => "Follow me",
            };
        }

        #endregion

        public static string GetActionText(LocalizedAction action)
        {
            return action switch
            {
                LocalizedAction.PICKUP_ITEM => PickupItem(),
                LocalizedAction.TALK => Talk(),
                _ => "",
            };
        }
        public static string GetStealChance(float chance)
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Roubar (%" + chance + " chance de sucesso)",
                _ => "Steal (%" + chance + " success probability)",
            };
        }
        public static string ExitShop()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Sair da loja",
                _ => "Exit Shop",
            };
        }
        public static string Bought()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Cacildes comprou",
                _ => "Bought",
            };
        }

        public static string Buy()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Comprar",
                _ => "Buy",
            };
        }

        public static string Sell()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Vender",
                _ => "Sell",
            };
        }

        public static string PickupItem()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Pegar item",
                _ => "Pickup item",
            };
        }

        public static string Unequip()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Desequipar",
                _ => "Unequip",
            };
        }

        public static string Back()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Voltar atr�s",
                _ => "Go Back",
            };
        }

        public static string Talk()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Conversar",
                _ => "Talk",
            };
        }

        public static string Found()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Cacildes encontrou",
                _ => "Found",
            };
        }

        public static string Used()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Cacildes usou",
                _ => "Used",
            };
        }

        public static string Read()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Ler",
                _ => "Read",
            };
        }
            
        public static string LearnedRecipe()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Receita aprendida: ",
                _ => "Learned recipe: ",
            };
        }

        public static string Cook()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Cozinhar",
                _ => "Cook",
            };
        }

        public static string UseAlchemyTable()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Usar mesa de alquimia",
                _ => "Use Alchemy Table",
            };
        }

        public static string AnvilTable()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Bigorna",
                _ => "Blacksmith Anvil",
            };
        }

        public static string UseBlacksmithAnvil()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Usar bigorna",
                _ => "Use Blacksmith Anvil",
            };
        }

        public static string UseLadder()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Usar escada",
                _ => "Use Ladder",
            };
        }

        public static string Previous()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Anterior",
                _ => "Previous",
            };
        }

        public static string Continue()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Continuar",
                _ => "Continue",
            };
        }

        public static string Next()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Pr�ximo",
                _ => "Next",
            };
        }

        #region Equipment Menu

        public static string ATK()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "ATQ",
                _ => "ATK",
            };
        }

        public static string Weapon()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Arma",
                _ => "Weapon",
            };
        }

        public static string Vitality()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Vitalidade",
                _ => "Vitality",
            };
        }

        public static string Intelligence()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Intelig�ncia",
                _ => "Intelligence",
            };
        }

        public static string Endurance()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Resist�ncia",
                _ => "Endurance",
            };
        }

        public static string Strength()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "For�a",
                _ => "Strength",
            };
        }

        public static string Dexterity()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Destreza",
                _ => "Dexterity",
            };
        }

        public static string Poise()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Postura",
                _ => "Poise",
            };
        }


        public static string ReputationIncreased(int amount)
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "A tua reputa��o subiu +" + amount + " pontos! Reputa��o atual: " + Player.instance.GetCurrentReputation(),
                _ => "Your reputation increased +" + amount + " points! Current reputation: " + Player.instance.GetCurrentReputation(),
            };
        }

        public static string ReputationDecreased(int amount)
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "A tua reputa��o desceu -" + amount + " pontos! Reputa��o atual: " + Player.instance.GetCurrentReputation(),
                _ => "Your reputation decreased -" + amount + " points! Current reputation: " + Player.instance.GetCurrentReputation(),
            };
        }

        public static string InsufficientReputationToEnterThisHouse()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Reputa��o insuficiente para entrar nesta casa",
                _ => "Insufficient reputation to enter this house",
            };
        }

        public static string Reputation()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Reputa��o",
                _ => "Reputation",
            };
        }

        public static string Gold()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Ouro",
                _ => "Gold",
            };
        }

        public static string YourCurrentGold()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Dinheiro Atual",
                _ => "Your Gold",
            };
        }

        public static string Coins()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Moedas",
                _ => "Coins",
            };
        }

        public static string Shield()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Escudo",
                _ => "Shield",
            };
        }

        public static string MagicDefense()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Defesa M�gica",
                _ => "Magic Defense",
            };
        }

        public static string Defense()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Defesa F�sica",
                _ => "Physical Defense",
            };
        }

        public static string FireDefense()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Defesa de Fogo",
                _ => "Fire Defense",
            };
        }

        public static string FrostDefense()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Defesa de Gelo",
                _ => "Frost Defense",
            };
        }

        public static string SpeedPenalty()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Perda de Velocidade",
                _ => "Speed Penalty",
            };
        }

        public static string PoiseBonus()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "B�nus Postura",
                _ => "Poise Bonus",
            };
        }

        public static string Resistence()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Resist�ncia",
                _ => "Resistence",
            };
        }

        public static string DEFAbsorption()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Absor��o de Dano",
                _ => "DEF Absorption",
            };
        }

        public static string Helmet()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Elmo",
                _ => "Helmet",
            };
        }

        public static string Armor()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Armadura",
                _ => "Armor",
            };
        }

        public static string Gauntlets()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Manoplas",
                _ => "Gauntlets",
            };
        }

        public static string Boots()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Botas",
                _ => "Boots",
            };
        }

        public static string Accessory()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Acess�rio",
                _ => "Accessory",
            };
        }

        public static string FavoriteItems()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Itens Favoritos",
                _ => "Favorite Items",
            };
        }

        public static string Gear()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Equipamento",
                _ => "Gear",
            };
        }

        #endregion

        #region Equipment Selection Menu

        public static string StaminaCostPerBlock()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Custo de stamina por bloqueio",
                _ => "Stamina Cost Per Block",
            };
        }

        public static string EnemyATKAbsorption()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Absor��o de ATQ do Inimigo",
                _ => "Enemy ATK Absorption",
            };
        }

        public static string ATKAbsorption()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Absor��o de ATQ",
                _ => "ATK Absorption",
            };
        }

        public static string Destroyed()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "destru�do",
                _ => "destroyed",
            };
        }

        public static string WasDestroyedByUnequiping()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "foi destru�do ao desequipar",
                _ => "was destroyed by unequiping it",
            };
        }

        public static string SpeedLoss()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Perda de velocidade",
                _ => "Speed Loss",
            };
        }

        public static string Value()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Valor",
                _ => "Value",
            };
        }

        public static string Buildup()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "B�nus",
                _ => "Buildup",
            };
        }

        public static string FrostBonus()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "B�nus Gelo",
                _ => "Frost Bonus",
            };
        }

        public static string LightningBonus()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "B�nus Rel�mpago",
                _ => "Lightning Bonus",
            };
        }

        public static string MagicBonus()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "B�nus de Magia",
                _ => "Magic Bonus",
            };
        }

        public static string FireBonus()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "B�nus Fogo",
                _ => "Fire Bonus",
            };
        }

        public static string BaseDamage()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Dano Base",
                _ => "Base Damage",
            };
        }

        public static string PhysicalDamage()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Dano F�sico",
                _ => "Physical Damage",
            };
        }

        public static string Equiped()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Equipado",
                _ => "Equiped",
            };
        }

        public static string Favorited()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Favoritado",
                _ => "Favorited",
            };
        }

        #endregion

        #region Inventory Menu

        public static string ShowAll()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Mostrar Tudo",
                _ => "All",
            };
        }

        public static string ShowConsumables()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Consum�veis",
                _ => "Consumables",
            };
        }

        public static string ShowAlchemyIngredients()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Ingredientes de Alquimia",
                _ => "Alchemy Ingredients",
            };
        }

        public static string ShowCookingIngredients()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Ingredientes de Cozinha",
                _ => "Cooking Ingredients",
            };
        }

        public static string ShowCraftingMaterials()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Min�rios",
                _ => "Ores",
            };
        }

        public static string ShowUpgradeMaterials()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Materiais de Ferreiro",
                _ => "Upgrade Materials",
            };
        }

        public static string ShowKeyItems()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Itens importantes",
                _ => "Key items",
            };
        }

        public static string UseItem()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Usar",
                _ => "Use",
            };
        }

        #endregion


        #region Load Screen Menu

        public static string Level()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "N�vel",
                _ => "Level",
            };
        }

        public static string ProgressSaved()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Jogo guardado",
                _ => "Game saved",
            };
        }
        public static string SavedAt()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Guardado a",
                _ => "Saved at",
            };
        }
        public static string TotalPlayTime()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Tempo total de jogo",
                _ => "Total play time",
            };
        }


        public static string LV()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Nv.",
                _ => "Lv.",
            };
        }

        public static string Load()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Carregar",
                _ => "Load",
            };
        }

        public static string Delete()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Apagar",
                _ => "Delete",
            };
        }

        #endregion


        public static string Loading()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "A carregar",
                _ => "Loading",
            };
        }

        public static string YouDied()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Cacildes ficou inconsciente",
                _ => "You Died",
            };
        }

        #region Player Actions
        public static string CantConsumeAtThisTime()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "N�o podes consumir itens neste momento",
                _ => "Can't consume item at this time",
            };
        }
        public static string DepletedConsumable()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Item depletado",
                _ => "Consumable depleted",
            };
        }
        public static string CantShootArrowsAtThisTime()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "N�o podes disparar o arco neste momento",
                _ => "Can't shoot arrows at this time",
            };
        }
        public static string BowRequired()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Um arco � necess�rio para disparar flechas",
                _ => "Bow required for shooting arrows",
            };
        }
        public static string CacildesLeveledUp()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Cacildes subiu de n�vel!",
                _ => "Cacildes leveled up!",
            };
        }
        #endregion

        #region Combat

        public static string From()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "De",
                _ => "From",
            };
        }

        public static string Poisoned()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Envenenado",
                _ => "Poisoned",
            };
        }

        public static string Bleeding()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Sangrando",
                _ => "Bleeding",
            };
        }

        public static string CriticalAttack()
        {
            return GamePreferences.instance.gameLanguage switch
            {
                GamePreferences.GameLanguage.PORTUGUESE => "Ataque Cr�tico",
                _ => "Critical Attack",
            };
        }

        #endregion
    }
}
