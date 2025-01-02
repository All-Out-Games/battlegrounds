13
107374182401
46145610696738 1723586970968342300
{
  "name": "Portal_AFK",
  "local_enabled": false,
  "local_position": {
    "X": -7.3054265975952148,
    "Y": -17.0000095367431641
  },
  "local_rotation": 0,
  "local_scale": {
    "X": 0.3000000119209290,
    "Y": 0.3000000119209290
  },
  "previous_sibling": "10214414044086:1716402952433790600",
  "next_sibling": "31069919089008:1716413396906995300",
  "parent": "359905312717597:1716331881724713300",
  "spawn_as_networked_entity": true
},
{
  "cid": 1,
  "aoid": "46145610892819:1723586970968420000",
  "component_type": "Internal_Component",
  "internal_component_type": "Spine_Animator",
  "data": {
    "skeleton_data_asset": "environment/CentralHub/Portals/animation/BAT003_portals.spine",
    "ordered_skins": [
      "hourglass"
    ],
    "depth_offset": 3.6600000858306885,
    "skeleton_scale": {
      "X": 2.4000000953674316,
      "Y": 2.4000000953674316
    }
  }
},
{
  "cid": 3,
  "aoid": "46145611064317:1723586970968488900",
  "component_type": "Internal_Component",
  "internal_component_type": "Interactable",
  "data": {
    "text": "Teleport to AFK"
  }
},
{
  "cid": 2,
  "aoid": "46145611255728:1723586970968565600",
  "component_type": "Mono_Component",
  "mono_component_type": "ZoneTeleporter",
  "data": {
    "ChangeStatusTo": "AFK"
  }
}
