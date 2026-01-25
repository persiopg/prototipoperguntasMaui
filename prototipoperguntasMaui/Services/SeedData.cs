namespace prototipoperguntasMaui.Services
{
    public static class SeedData
    {
        public const string JsonContent = """
{
  "app_metadata": {
    "cycle_id": "C1_2026",
    "description": "Ciclo 1: Jan, Fev, Mar 2026 - Trade Force",
    "version": "1.0.1",
    "last_update": "2026-01-24T12:00:00Z"
  },
  "stores_mock": [
    {
      "id": 101,
      "name": "FARMACIA SAO JOAO",
      "channel": "FARMA",
      "address": "Av. Presidente Vargas, 1200, Jd. São Luiz, Ribeirão Preto - SP",
      "lat": -21.19310,
      "lng": -47.81450
    },
    {
      "id": 202,
      "name": "SUPERMERCADO EXTRA",
      "channel": "ALIMENTAR",
      "address": "Av. Maurílio Biagi, 800, Santa Cruz, Ribeirão Preto - SP",
      "lat": -21.20500,
      "lng": -47.79800
    },
    {
      "id": 303,
      "name": "PERFUMARIA BELA",
      "channel": "PERFUMARIA",
      "address": "Av. Cel. Fernando Ferreira Leite, 1540 (RibeirãoShopping), Ribeirão Preto - SP",
      "lat": -21.21200,
      "lng": -47.82500
    }
  ],
  "scoring_rules": {
    "weights": { "PRESENCA": 50, "POSICIONAMENTO": 30, "PRECO": 20 },
    "targets_by_channel": [
      {"pillar": "PRESENCA", "target": 40, "channel": "FARMA", "obs": "Baseline"},
      {"pillar": "PRESENCA", "target": 10, "channel": "ALL", "obs": "Lançamentos"},
      {"pillar": "POSICIONAMENTO", "target": 20, "channel": "ALL", "obs": "Ponto Natural"},
      {"pillar": "POSICIONAMENTO", "target": 10, "channel": "ALL", "obs": "Ponto Extra"}
    ]
  },
  "questions_catalog": [
    {
      "id": "Q_POS_001",
      "sub_pillar": "Posicionamento Ponto Natural",
      "brand": "AUSTRALIAN GOLD",
      "text": "Os promopacks estão com VB de 3 unidades cada?",
      "guidance": "Garantir abast. do pack",
      "points_config": { "farma": 3, "alimentar": 1.5, "perfumaria": 0, "depto": 0 }
    },
    {
      "id": "Q_POS_002",
      "sub_pillar": "Posicionamento Ponto Natural",
      "brand": "AUSTRALIAN GOLD",
      "text": "O item de Bronzeado Protegido FPS30 237gr está com VB de 3 unidades cada?",
      "guidance": "Garantir abast. do principal FPS",
      "points_config": { "farma": 3, "alimentar": 1.5, "perfumaria": 0, "depto": 0 }
    },
    {
      "id": "Q_POS_003",
      "sub_pillar": "Posicionamento Ponto Natural",
      "brand": "NIINA SECRETS MAKE",
      "text": "O VB da Base Hidra Glow está com no mínimo 3 unidades cada?",
      "guidance": "Garantir abastecimento principal SKU",
      "points_config": { "farma": 0, "alimentar": 0, "perfumaria": 3, "depto": 5 }
    },
    {
      "id": "Q_POS_004",
      "sub_pillar": "Posicionamento Ponto Natural",
      "brand": "PAMPERS",
      "text": "Os segmentos de sabonete e shampoo estão agrupados na exposição?",
      "guidance": "Garantir destaque maiores segmentos",
      "points_config": { "farma": 1, "alimentar": 2, "perfumaria": 0, "depto": 0 }
    },
    {
      "id": "Q_POS_005",
      "sub_pillar": "Posicionamento Ponto Natural",
      "brand": "PAMPERS",
      "text": "A marca está ao lado do nosso principal concorrente Johnson & Johnson?",
      "guidance": "Garantir posicionamento próx concorrência",
      "points_config": { "farma": 1, "alimentar": 1, "perfumaria": 0, "depto": 0 }
    },
    {
      "id": "Q_POS_EXTRA_GENERIC",
      "sub_pillar": "Posicionamento Ponto Extra",
      "brand": "GENERICO",
      "text": "Qual tipo de Ponto Extra foi encontrado?",
      "options": ["PDG MISTA", "SIAGE", "AG", "VULT CABELOS"],
      "guidance": "Identificar conquista adicional",
      "points_config": { "farma": 10, "alimentar": 10, "perfumaria": 10, "depto": 10 }
    },
    {
      "id": "Q_LANC_001",
      "sub_pillar": "Presença Lançamento",
      "brand": "SIAGE",
      "text": "Há presença da linha NutriAcid no PDV?",
      "guidance": "Garantir a presença do lançamento prioritário",
      "points_config": { "farma": 7, "alimentar": 7, "perfumaria": 7, "depto": 7 }
    },
    {
      "id": "Q_LANC_002",
      "sub_pillar": "Presença Lançamento",
      "brand": "SIAGE",
      "text": "Há presença da linha Volume Imediato no PDV?",
      "guidance": "Garantir a presença do lançamento prioritário (Março)",
      "points_config": { "farma": 7, "alimentar": 7, "perfumaria": 7, "depto": 7 }
    },
    {
      "id": "Q_LANC_VB",
      "sub_pillar": "Presença Lançamento",
      "brand": "SIAGE",
      "text": "Qual o nível de abastecimento (VB) do Lançamento?",
      "options": ["Até 2 un (0pts)", "3-5 un", "+6 un"],
      "guidance": "Garantir VB > 3 unidades",
      "points_config": { "farma": 3, "alimentar": 3, "perfumaria": 3, "depto": 3 }
    },
    {
      "id": "Q_POS_006",
      "sub_pillar": "Posicionamento Ponto Natural",
      "brand": "SIAGE",
      "text": "O lançamento do ciclo Nutri Acid ou Volume Imediato está na altura dos olhos?",
      "guidance": "Garantir destaque lançamento",
      "points_config": { "farma": 5, "alimentar": 5, "perfumaria": 6, "depto": 6 }
    },
    {
      "id": "Q_POS_007",
      "sub_pillar": "Posicionamento Ponto Natural",
      "brand": "SIAGE",
      "text": "As linhas de Nutri Rose, Hair Plastia, Nutri Acid estão agrupadas?",
      "guidance": "Destacar power SKUs",
      "points_config": { "farma": 5, "alimentar": 4, "perfumaria": 6, "depto": 6 }
    },
    {
      "id": "Q_POS_008",
      "sub_pillar": "Posicionamento Ponto Natural",
      "brand": "VULT CABELOS",
      "text": "As linhas de Hidratação e Liso estão próximas na exposição?",
      "guidance": "Destacar power SKUs",
      "points_config": { "farma": 0, "alimentar": 5, "perfumaria": 0, "depto": 0 }
    },
    {
      "id": "Q_POS_009",
      "sub_pillar": "Posicionamento Ponto Natural",
      "brand": "VULT MAKE",
      "text": "O VB de máscara de cílios está com no mínimo 3 unidades cada?",
      "guidance": "Garantir abastecimento",
      "points_config": { "farma": 2, "alimentar": 0, "perfumaria": 5, "depto": 3 }
    }
  ],
  "campaigns_catalog": [
    { "id": "CMP_001", "type": "MPDV", "channel": "ALIMENTAR", "task": "FAIXA DE GÔNDOLA - MPDV - EUDORA SIAGE", "dates": { "start": "2026-01-01", "end": "2026-04-01" } },
    { "id": "CMP_002", "type": "MPDV", "channel": "ALIMENTAR", "task": "RÉGUA DE DANOS - MPDV - EUDORA SIAGE", "dates": { "start": "2026-01-01", "end": "2026-04-01" } },
    { "id": "CMP_003", "type": "MPDV", "channel": "FARMA", "task": "KIT PDG (faixas + wobbler) - MPDV - EUDORA SIAGE", "dates": { "start": "2026-01-01", "end": "2026-04-01" } },
    { "id": "CMP_004", "type": "MPDV", "channel": "FARMA", "task": "GLORIFIER ÓLEOS EXCLUSIVOS RD - EUDORA SIAGE", "dates": { "start": "2026-01-01", "end": "2026-04-01" } },
    { "id": "CMP_005", "type": "MPDV", "channel": "PERFUMARIA", "task": "BANDEJA EXPOSITORA PALETTE SHINE FLOWERS - NIINA SECRETS", "dates": { "start": "2026-01-01", "end": "2026-04-01" } },
    { "id": "CMP_006", "type": "CONCORRENCIA", "channel": "TODOS", "task": "Há Ponto Extra de Cabelos de L'oréal no PDV? Quantos?", "dates": { "start": "2026-01-01", "end": "2026-04-01" } }
  ],
  "products_catalog": [
    { "ean": "7891033551538", "name": "PROMOPACK SIÀGE NUTRI ROSE + LEAVE IN", "brand": "SIAGE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033534357", "name": "SHAMPOO SIAGE NUTRI ROSE 250ML", "brand": "SIAGE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033534364", "name": "CONDICIONADOR SIAGE NUTRI ROSE 200ML", "brand": "SIAGE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033534371", "name": "MASCARA SIAGE NUTRI ROSE 250G", "brand": "SIAGE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033581214", "name": "MASCARA SIAGE NUTRI ACID 250G", "brand": "SIAGE", "tag": "LANCAMENTO", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033581191", "name": "SHAMPOO SIAGE NUTRI ACID 250ML", "brand": "SIAGE", "tag": "LANCAMENTO", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033581207", "name": "CONDICIONADOR SIAGE NUTRI ACID 200ML", "brand": "SIAGE", "tag": "LANCAMENTO", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033016419", "name": "SHAMPOO SIAGE HAIR-PLASTIA 250ML", "brand": "SIAGE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033016440", "name": "MASCARA SIAGE HAIR-PLASTIA 250G", "brand": "SIAGE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033761104", "name": "KIT AG PROTETOR FPS 30 + ACELERADOR", "brand": "AG", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7891033473151", "name": "KIT AG PROTETOR CORP FPS 50 + FACIAL", "brand": "AG", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7899852022642", "name": "PÓ COMPACTO VULT V420", "brand": "VULT MAKE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7899852022659", "name": "PÓ COMPACTO VULT V430", "brand": "VULT MAKE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7899852024035", "name": "MÁSCARA CÍLIOS VULT VOLUME UP", "brand": "VULT MAKE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7899852022543", "name": "BASE MATTE HIDRALURONIC V200", "brand": "VULT MAKE", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "PERFUMARIA", "DEPTO", "ALIMENTAR"] },
    { "ean": "7899852025452", "name": "SHAMPOO VULT CABELOS LISO 350ML", "brand": "VULT CABELOS", "rules": { "presence": true, "price": true }, "channels": ["ALIMENTAR", "FARMA"] },
    { "ean": "7899852025506", "name": "CONDICIONADOR VULT CABELOS LISO 325ML", "brand": "VULT CABELOS", "rules": { "presence": true, "price": true }, "channels": ["ALIMENTAR", "FARMA"] },
    { "ean": "7891033932757", "name": "BASE NIINA SECRETS HD/GLOW COR 05", "brand": "NIINA SECRETS MAKE", "rules": { "presence": true, "price": true }, "channels": ["PERFUMARIA", "DEPTO"] },
    { "ean": "7891033540037", "name": "BATOM LIQ NIINA SECRETS SKINNY MATTE HIBISCO", "brand": "NIINA SECRETS MAKE", "rules": { "presence": true, "price": true }, "channels": ["PERFUMARIA", "DEPTO"] },
    { "ean": "7891033519286", "name": "PAMPERS SAB LIQ GLICERINA 200ML", "brand": "PAMPERS", "rules": { "presence": true, "price": true }, "channels": ["FARMA", "ALIMENTAR"] }
  ]
}
""";
    }
}
