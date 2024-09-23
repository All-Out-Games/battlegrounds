13
395136991233
554562575411248 1727045409107853100
{
  "name": "CrateItemDrop",
  "local_enabled": true,
  "local_position": {

  },
  "local_rotation": 0,
  "local_scale": {
    "X": 2,
    "Y": 2
  }
},
{
  "cid": 1,
  "aoid": "554562575500484:1727045409107896900",
  "component_type": "Internal_Component",
  "internal_component_type": "Sprite_Renderer",
  "data": {
    "texture": "Props/DropItems/HealthPotionM.png",
    "depth_offset": 0.3300000131130219
  }
},
{
  "cid": 2,
  "aoid": "554610769085288:1727045433243300900",
  "component_type": "Mono_Component",
  "mono_component_type": "CrateItemDrop",
  "data": {
    "_pickupTrigger": "554624595218512:1727045440167444500",
    "_renderer": "554562575500484:1727045409107896900",
    "Fade": "554615898327806:1727045435812031700"
  }
},
{
  "cid": 3,
  "aoid": "554615898327806:1727045435812031700",
  "component_type": "Mono_Component",
  "mono_component_type": "FadeAfterStart",
  "data": {
    "FadeSprite": true,
    "PersistTime": 5,
    "FadeTime": 6
  }
},
{
  "cid": 4,
  "aoid": "554624595218512:1727045440167444500",
  "component_type": "Internal_Component",
  "internal_component_type": "Circle_Collider",
  "data": {
    "size": 0.8000000119209290,
    "is_trigger": true
  }
}
