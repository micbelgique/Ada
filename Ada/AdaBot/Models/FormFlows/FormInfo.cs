using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdaBot.Models.FormFlows
{
    public enum StatusOptions { test, Un_visiteur, Un_étudiant, Un_développeur, Un_porteur_de_projet };
    public enum QuestionVisitOptions
    {
        test,
        Que_fait_le_MIC, 
        Que_faire_au_MIC,
        Quels_sont_les_horaires_du_MIC
    };
    public enum QuestionStudentOptions
    {
        test,
        Que_fait_le_MIC,
        Que_faire_au_MIC,
        Quels_sont_les_horaires_du_MIC,
        Comment_postuler_pour_un_stage,
        Comment_se_déroule_un_stage
    };
    public enum QuestionDevOptions
    {
        test,
        Que_fait_le_MIC,
        Que_faire_au_MIC,
        Quels_sont_les_horaires_du_MIC,
        Du_travail_pour_moi
    };
    public enum QuestionProjectOptions
    {
        test,
        Que_fait_le_MIC,
        Que_faire_au_MIC,
        Quels_sont_les_horaires_du_MIC,
        Comment_travailler_ici,
        Comment_louer_une_salle,
        A_qui_parler_de_mon_projet,
        Comment_se_passe_mon_accompagnement_dans_mon_projet
    };

    [Serializable]
    public class FormInfo
    {
        [Prompt("Qui est-tu? {||}")]
        public StatusOptions Status { get; set; }
        [Prompt("Quelle est ta question? {||}")]
        public QuestionVisitOptions QuestionVisit { get; set; }
        [Prompt("Quelle est ta question? {||}")]
        public QuestionStudentOptions QuestionStudent { get; set; }
        [Prompt("Quelle est ta question? {||}")]
        public QuestionDevOptions QuestionDev { get; set; }
        [Prompt("Quelle est ta question? {||}")]
        public QuestionProjectOptions QuestionProject { get; set; }

        public static IForm<FormInfo> BuildForm()
        {
            return new FormBuilder<FormInfo>()
                .Field(nameof(QuestionVisit), IsVisit)
            .Field(nameof(QuestionStudent), IsStudent)
            .Field(nameof(QuestionDev), IsDev)
            .Field(nameof(QuestionProject), IsProject)
            .AddRemainingFields()
                    .Build();
        }

        private static bool IsStudent(FormInfo state)
        {
            return state.Status == StatusOptions.Un_étudiant;
        }
        private static bool IsDev(FormInfo state)
        {
            return state.Status == StatusOptions.Un_développeur;
        }
        private static bool IsProject(FormInfo state)
        {
            return state.Status == StatusOptions.Un_porteur_de_projet;
        }
        private static bool IsVisit(FormInfo state)
        {
            return state.Status == StatusOptions.Un_visiteur;
        }
    }
}